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
    public class TesterControllerUT
    {
        [TestMethod]
        public void PostUser_ReturnOkAndUser()
        {
            // Arrange
            var controller = new TesterController();

            // Act
            UserPOST post = new UserPOST { Username = "alfredo@mail.com", Password = "vargas" };
            var actionResult = controller.Post(post);
            var contentResult = actionResult as OkNegotiatedContentResult<ResponsePOCO>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<ResponsePOCO>));
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(post, contentResult.Content.Data);
        }

        [TestMethod]
        public void PostUser_ReturnErrorWhenNull()
        {
            // Arrange
            var controller = new TesterController();

            // Act
            UserPOST post = null;
            var actionResult = controller.Post(post);
            var contentResult = actionResult as NotFoundResult;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }
    }
}
