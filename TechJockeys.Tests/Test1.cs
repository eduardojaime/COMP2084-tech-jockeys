using System.ComponentModel.DataAnnotations;

namespace TechJockeys.Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange - declare any variables needed for the test
            var a = 2;
            var b = 2;
            var expected = 4;

            // Act - perform the action or call the method being tested
            // This is the business logic getting tested "adding two numbers"
            var actual = a + b;

            // Assert - verify the expected outcome using assertions
            Assert.IsInstanceOfType<int>(actual);
            Assert.AreEqual(expected, actual);            
        }
    }
}
