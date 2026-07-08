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
    }
}
