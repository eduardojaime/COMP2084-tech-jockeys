using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechJockeys.Data;
using TechJockeys.Models;

namespace TechJockeys.Controllers
{
    public class ProductsController : Controller
    {
        // shared db obj
        private readonly ApplicationDbContext _context;

        // constructor w/db dependency
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // product list to pass for display in view
            var products = _context.Product.ToList();

            return View(products);
        }

        // GET: /Products/Create => show empty Product form including Category list dropdown
        public IActionResult Create()
        {
            // fetch Categories for dropdown, ordered a-z by Name
            ViewBag.CategoryId = new SelectList(_context.Category.OrderBy(c => c.Name).ToList(), "CategoryId", "Name");
            return View();
        }

        // POST: /Products/Create => save new product from form data
        [HttpPost]
        public IActionResult Create([Bind("Name,Price,Stock,Description,Image,CategoryId")] Product product)
        {
            // validate
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // create & save
            _context.Product.Add(product);
            _context.SaveChanges();

            // redirect to list
            return RedirectToAction("Index");
        }

        // GET: /Products/Edit/5 => show populated product form
        public IActionResult Edit(int id)
        {
            // find product by id
            var product = _context.Product.Find(id);

            // if not found => error
            if (product == null)
            {
                return NotFound();
            }

            // fetch Categories for dropdown, ordered a-z by Name
            ViewBag.CategoryId = new SelectList(_context.Category.OrderBy(c => c.Name).ToList(), "CategoryId", "Name");

            // pass product data to view for display
            return View(product);
        }

        // POST: /Products/Edit/5 => update product from form values
        [HttpPost]
        public IActionResult Edit([Bind("ProductId,Name,Price,Stock,Description,Image,CategoryId")] Product product)
        {
            // input validation
            if (!ModelState.IsValid)
            {
                // invalid => reload page w/existing values
                return View(product);
            }

            // data valid => save to db
            _context.Product.Update(product);
            _context.SaveChanges();

            // refresh index list
            return RedirectToAction("Index");
        }

        // GET: /Products/Delete/5 => find & delete selected product
        public IActionResult Delete(int id)
        {
            // find product by id
            var product = _context.Product.Find(id);

            // if not found => error
            if (product == null)
            {
                return NotFound();
            }

            // remove from db
            _context.Product.Remove(product);
            _context.SaveChanges();

            // refresh list on index
            return RedirectToAction("Index");
        }
    }
}
