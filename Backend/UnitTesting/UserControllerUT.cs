using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApi_PointMap.Controllers;
using WebApi_PointMap.Models;

namespace UnitTesting
{
    [TestClass]
    public class UserControllerUT
    {
        [TestMethod]
        public void GetUser()
        {
            // Arrange
            var controller = new UserController();
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            // Act
            var response = controller.Get(10);

            // Assert
            var expectedResult = "alfredo";
            var result = controller.Get() as OkNegotiatedContentResult<string>;
            //Assert.AreEqual(expectedResult, result.Content.);
        }
    }
}
