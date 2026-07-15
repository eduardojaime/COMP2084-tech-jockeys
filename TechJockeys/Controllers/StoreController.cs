using Microsoft.AspNetCore.Mvc;
using TechJockeys.Data;
using TechJockeys.Models;

namespace TechJockeys.Controllers
{
    public class StoreController : Controller
    {
        // shared db conn
        private readonly ApplicationDbContext _context;

        // constructor w/db conn dependency
        public StoreController(ApplicationDbContext context)
        {
            _context = context;
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
            if (category == null) {
                return RedirectToAction("index");
            }

            // use id param to find category
            // use ViewData dictionary to show selected category name in heading
            // since category is nullable, question mark '?' will make this value empty on runtime if null
            ViewData["Category"] = category?.Name;
      
            return View(products);
        }

        [HttpPost]
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity) {
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
            return "123"; // Placeholder for customer ID retrieval logic
        }
    }
}
