using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using TechJockeys.Data;
using TechJockeys.Extensions;
using TechJockeys.Models;

namespace TechJockeys.Controllers
{
    public class StoreController : Controller
    {
        // shared db conn
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // constructor w/db conn dependency
        public StoreController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // fetch category data from db
            var categories = _context.Category.OrderBy(c => c.Name).ToList();

            // load view and pass the category list
            return View(categories);
        }

        // This method returns a page with the products belonging to that category
        public IActionResult ByCategory(int id)
        {

            // error handle if id missing => redirect to Store index so user can choose a category
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }

            // Retrieve list of products from the DB
            var products = _context.Product                         // FROM PRODUCT p
                                    .Where(p => p.CategoryId == id) // WHERE p.CategoryId = @id 
                                    .OrderBy(p => p.Name)           // ORDER BY p.Name
                                    .ToList();                      // SELECT *

            // Retrieve the category name to show on the page in the title
            var category = _context.Category.Find(id);
            // redirect to index if category is null
            if (category == null)
            {
                return RedirectToAction("index");
            }

            // use id param to find category
            // use ViewData dictionary to show selected category name in heading
            // since category is nullable, question mark '?' will make this value empty on runtime if null
            ViewData["Category"] = category?.Name;

            return View(products);
        }

        [HttpPost]
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity)
        {
            // get userId or generate temp id for not-logged in users
            // retrieve from session storage storage
            var customerId = GetCustomerId();

            // Validate the product ID and quantity
            if (Quantity <= 0 || ProductId <= 0)
            {
                return BadRequest("Invalid quantity or product ID.");
            }
            // Validate that the product exists in the DB
            var product = _context.Product.Find(ProductId);
            if (product == null)
            {
                return NotFound("Invalid product ID.");
            }

            // Get product price
            var price = product.Price;

            // Create new cart record
            var cartItem = new CartItem
            {
                Quantity = Quantity,
                Price = price,
                ProductId = ProductId,
                CustomerId = customerId
            };

            // Save new cart item to the database
            _context.CartItem.Add(cartItem); // at this point, the cart item is only in memory not yet
            _context.SaveChanges(); // this is when the new record is actually saved to the database

            // Redirect to Cart view to show the user's cart
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult Cart()
        {
            // get user id so we can filter car items
            var customerId = GetCustomerId();
            // get cart items associated to that user id
            var cartItems = _context.CartItem                               // SELECT * FROM CartItem ci
                                .Include(ci => ci.Product)                  // JOIN Product p ON ci.ProductId = p.ProductId
                                .Where(ci => ci.CustomerId == customerId)   // WHERE ci.CustomerId = @customerId
                                .OrderBy(ci => ci.CartItemId)               // ORDER BY ci.CartItemId
                                .ToList();

            // calculate total price for all items in the cart and pass as viewbag to the view
            ViewBag.TotalAmount = cartItems.Sum(ci => ci.Price * ci.Quantity);

            // return the view with the cart items model (list)
            return View(cartItems);
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int id)
        { 
            // Validate
            if (id <= 0)
            {
                return BadRequest("Invalid cart item ID.");
            }
            // find the cart item in the DB
            var cartItem = _context.CartItem.Find(id);

            // remove it from collection and save changes
            _context.CartItem.Remove(cartItem);
            _context.SaveChanges();

            // redirect back to cart view
            return RedirectToAction("Cart");
        }

        [HttpGet]
        [Authorize] // only logged-in users can access checkout
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [Authorize] // only logged-in users can access checkout
        public IActionResult Checkout(
            [Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order) {
            // Programmatically handle OrderDate, OrderTotal, and CustomerId
            order.OrderDate = DateTime.UtcNow; // best practice use UTC and convert to local time
            order.CustomerId = GetCustomerId();
            order.OrderTotal = _context.CartItem
                .Where(ci => ci.CustomerId == order.CustomerId)
                .Sum(ci => ci.Price * ci.Quantity);

            // Store order in session storage for later use in the confirmation page
            HttpContext.Session.SetObject("Order", order);

            // Send user to payment page
            return RedirectToAction("Payment");
        }

        // GET /Store/Payment
        [HttpGet]
        [Authorize] // only logged-in users can access payment
        public IActionResult Payment() {
            // retrieve order from session storage
            var order = HttpContext.Session.GetObject<Order>("Order");
            // pass the order to viewbag object
            ViewBag.TotalAmount = order.OrderTotal;
            // return the view
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult ProcessPayment() {
            // retrieve order from session storage
            var order = HttpContext.Session.GetObject<Order>("Order");
            // Set secret key
            StripeConfiguration.ApiKey = _configuration["Payments:Stripe:SecretKey"];
            // process payment using Stripe
            var domain = $"https://{Request.Host}";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmountDecimal = order.OrderTotal * 100, // Stripe expects amount in cents
                      Currency = "cad",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "TechJockeys Online Purchase",
                        },
                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                PaymentMethodTypes = new List<string> { "card" },
                SuccessUrl = domain + "/Shop/SaveOrder",
                CancelUrl = domain + "/Store/Cart"
            };
            var service = new SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); // redirect to Stripe checkout
        }

        // Helper Methods
        /// <summary>
        /// This method retrieves the customer ID from the session or generates a temporary ID 
        /// for not-logged-in users
        /// </summary>
        /// <returns>
        /// The customer ID as a string. It can be either a GUID or an email address.
        /// </returns>
        private string GetCustomerId()
        {
            // retrieve customer ID from session storage
            var customerId = HttpContext.Session.GetString("CustomerId");
            // handle if it's null or empty (not logged in, first-time visitor)
            if (string.IsNullOrEmpty(customerId))
            {
                // there's nothing in the session yet, so generate or get from user object and store
                if (User.Identity.IsAuthenticated)
                {
                    // user is logged in, use their email as the customer ID
                    customerId = User.Identity.Name; // this is usually the email address
                }
                else
                {
                    // user is not logged in, generate a temporary GUID for this session
                    customerId = Guid.NewGuid().ToString();
                }
                // Store whichever value we got in the session for future requests
                HttpContext.Session.SetString("CustomerId", customerId);
            }

            return customerId; // Placeholder for customer ID retrieval logic
        }
    }
}
