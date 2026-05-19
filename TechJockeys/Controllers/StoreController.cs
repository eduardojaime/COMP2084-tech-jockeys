using Microsoft.AspNetCore.Mvc;
using TechJockeys.Models;

namespace TechJockeys.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            // fetch category data (mock data today, from db later)
            var categories = new List<Category>();

            // create mock category list
            for (int i = 1; i < 16; i++)
            {
                categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            }

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
