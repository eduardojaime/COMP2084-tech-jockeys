using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TechJockeys.Data;
using TechJockeys.Models;

namespace TechJockeys.Controllers
{
    [Authorize(Roles = "Administrator")] // restrict all methods to Administrator role only
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

        // GET: /Categories/Edit/5 => fetch & display selected category
        public IActionResult Edit(int id)
        {
            // fetch category by id
            var category = _context.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            // pass selected category to view for display in the form
            return View(category);
        }

        // POST: /Categories/Edit/5 => update selected category from form submission
        [HttpPost]
        public IActionResult Edit([Bind("CategoryId,Name")] Category category)
        {
            // validate form inputs
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // update db
            _context.Category.Update(category);
            _context.SaveChanges();

            // redirect to list
            return RedirectToAction("Index");
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

            // check for child products
            if (category.Products == null)
            {
                return View("Error");
            }

            // delete from db
            _context.Category.Remove(category);
            _context.SaveChanges();

            // refresh list
            return RedirectToAction("Index");
        }
    }
}
