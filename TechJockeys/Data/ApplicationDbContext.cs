using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechJockeys.Models;

namespace TechJockeys.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TechJockeys.Models.Category> Category { get; set; } = default!;
        public DbSet<TechJockeys.Models.Product> Product { get; set; } = default!;
        public DbSet<TechJockeys.Models.CartItem> CartItem { get; set; } = default!;
        public DbSet<TechJockeys.Models.Order> Order { get; set; } = default!;
        public DbSet<TechJockeys.Models.OrderItem> OrderItem { get; set; } = default!;
    }
}
