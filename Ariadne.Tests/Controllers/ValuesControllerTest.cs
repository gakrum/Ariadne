using Ariadne.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ariadne.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void Post()
        {
            // Arrange
            MazeController controller = new MazeController();

            // Act
            controller.Post("value");

            // Assert
        }
    }
}
