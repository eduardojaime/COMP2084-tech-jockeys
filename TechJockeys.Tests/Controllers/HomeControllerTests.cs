using Microsoft.AspNetCore.Mvc;
using TechJockeys.Controllers;

namespace TechJockeys.Tests;

// Right click in the Controllers folder > Add new item > MSTest Test Class > name it HomeControllerTests.cs

[TestClass]
public class HomeControllerTests
{
    [TestMethod]
    public void IndexReturnsView()
    {
        // Arrange
        // Instance of the class that has the methods to test
        HomeController controller = new HomeController(null);
        // Act
        var result = controller.Index();
        // Assert
        Assert.IsInstanceOfType<ViewResult>(result);
    }
}
