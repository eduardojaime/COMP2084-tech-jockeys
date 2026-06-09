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

        public IActionResult ByCategory(int id)
        {
            // error handle if id missing => redirect to Store index so user can choose a category
            if (id == 0 || id > 15)
            {
                return RedirectToAction("Index");
            }

            // use id param to find category
            // use ViewData dictionary to show selected category name in heading
            ViewData["Category"] = "Category " + id.ToString();
      
            return View();
        }
    }
}
