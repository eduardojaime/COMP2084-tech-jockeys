using Microsoft.AspNetCore.Mvc;
using TechJockeys.Data;
using TechJockeys.Models;

namespace TechJockeys.Controllers
{
    public class CategoriesController : Controller
    {
        // shared db conn available to all controller methods
        private readonly ApplicationDbContext _context;

        // constructor w/dp dependency
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // fetch Category list from db using DbSet object
            var categories = _context.Category.ToList();

            // pass empty list to view
            return View(categories);
        }

        // GET: /Categories/Create => show empty Category form
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create => process form submission to create new Category
        [HttpPost]
        public IActionResult Create([Bind("Name")] Category category)
        {
            // validate input.  Show form again if invalid
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create new category & save to db
            _context.Category.Add(category);
            _context.SaveChanges();

            // redirect to Index to see updated list of Categories
            return RedirectToAction("Index");
        }

        public IActionResult Edit()
        {
            return View();
        }

        // GET: /Categories/Delete/5 => delete selected category
        public IActionResult Delete(int id)
        {
            // find Category to delete
            var category = _context.Category.Find(id);

            if (category == null)
            {
                // return RedirectToAction("Index");
                return NotFound();
            }

            // delete from db
            _context.Category.Remove(category);
            _context.SaveChanges();

            // refresh list
            return RedirectToAction("Index");
        }
    }
}
