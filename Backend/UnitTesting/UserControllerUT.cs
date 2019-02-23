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
        public void PostUser_ReturnOkAndUser()
        {
            // Arrange
            var controller = new UserController();

            // Act
            UserPOST post = new UserPOST { Username = "alfredo@mail.com", Password = "vargas" };
            var actionResult = controller.Post(post);
            var contentResult = actionResult as OkNegotiatedContentResult<UserPOST>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<UserPOST>));
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(post.Username, contentResult.Content.Username);
        }

        [TestMethod]
        public void PostUser_ReturnErrorWhenNull()
        {
            // Arrange
            var controller = new UserController();

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
