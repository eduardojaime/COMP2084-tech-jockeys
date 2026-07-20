
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechJockeys.Models;
using TechJockeys.Data;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ORDERS
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Order
            .Where(o => o.CustomerId == User.Identity.Name)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }

    // GET: ORDERS/Details/5
    public async Task<IActionResult> Details(int? orderid)
    {
        if (orderid == null)
        {
            return NotFound();
        }

        var order = await _context.Order
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(m => m.OrderId == orderid);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
