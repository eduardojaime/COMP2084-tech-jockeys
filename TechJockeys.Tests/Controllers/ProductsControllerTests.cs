using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechJockeys.Controllers;
using TechJockeys.Data;
using TechJockeys.Models;

namespace TechJockeys.Tests;

[TestClass]
public sealed class ProductsControllerTests
{
    // Declare any variables needed for the test class
    // ProductsController instance will be used by every method below
    private ProductsController _controller;
    private ApplicationDbContext _context;

    [TestInitialize]
    public void TestInitialize()
    {
        // This method automatically runs before each test method in the class. Use it to set up any common test data or state.
        // Arrange - declare any variables needed for the test
        // DB setup
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MockDB_{Guid.NewGuid()}") // << instead of connecting to sql server we use in memory database
            .Options;

        _context = new ApplicationDbContext(options);

        // At this point the DB is empty, so we can add some mock data to it for testing purposes
        var cat1 = new Category { CategoryId = 1, Name = "Monitors" };
        var cat2 = new Category { CategoryId = 2, Name = "Laptops" };

        _context.Category.AddRange(cat1, cat2); // << Add range is more efficient than adding one by one

        var prod1 = new Product { ProductId = 1, Name = "Dell Monitor", Description = "ABC", Price = 200, CategoryId = 1 };
        var prod2 = new Product { ProductId = 2, Name = "HP Laptop", Description = "ABC", Price = 800, CategoryId = 2 };
        var prod3 = new Product { ProductId = 3, Name = "Samsung Monitor", Description = "ABC", Price = 250, CategoryId = 1 };

        _context.Product.AddRange(prod1, prod2, prod3);
        _context.SaveChanges();

        // Controller instance
        _controller = new ProductsController(_context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Opposite of TestInitialize, this method runs after each test method in the class.
        // Use it to clean up any resources or reset state.
    }

    [TestMethod]
    public void IndexReturnsProductsList()
    {
        // Arrange - declare any variables needed for the test
        // Controller instance, mock db context, etc. > Apply to all tests so move them to a [TestInitialize] method
        // Keep anything that applies only to this tests in this section
        var expectedCount = _context.Product.Count(); // or just hardcode = 3

        // Act - perform the action or call the method being tested
        var result = _controller.Index() as ViewResult; // << convert to ViewResult so we can access the Model property and retrieve the list
        var model = result?.Model as List<Product>; // << convert to List<Product> so we can count the items
        var actualCount = model?.Count;

        // Assert - verify the expected outcome using assertions
        // check for null lists (something wrong with the controller logic)
        Assert.IsNotNull(model);
        // check that the count of products returned by the controller matches the expected count
        Assert.AreEqual(expectedCount, actualCount);
    }

    [TestMethod]
    public void DetailsReturnsValidProductWhenIdExists()
    {
        // Arrange
        var validId = 1; // Dell Monitor
        // Act 
        var result = _controller.Details(validId) as ViewResult;
        var model = result?.Model as Product;

        // Assert
        Assert.IsNotNull(model);
        Assert.Contains("dell", model?.Name.ToLower());
    }

    [TestMethod]
    public void DetailsReturnsEmptyModelWhenIdDoesNotExist()
    {
        // Arrange 
        var invalidId = 999;
        // Act
        var result = _controller.Details(invalidId) as ViewResult;
        var model = result?.Model as Product;
        // Assert
        Assert.IsNull(model);
    }
}
