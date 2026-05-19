using Microsoft.AspNetCore.Mvc;
using TechJockeys.Models;

namespace TechJockeys.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            // product list to pass for display in view
            var products = new List<Product>();

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
