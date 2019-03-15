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
        //[TestMethod]
        //public void PostUser_ReturnOkAndUser()
        //{
        //    // Arrange
        //    var controller = new TesterController();

        //    // Act
        //    UserPOSTDTO post = new UserPOSTDTO { Username = "alfredo@mail.com", Password = "vargas" };
        //    var actionResult = controller.Post(post);
        //    var contentResult = actionResult as OkNegotiatedContentResult<ResponseDTO>;

        //    // Assert
        //    Assert.IsNotNull(contentResult);
        //    Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<ResponseDTO>));
        //    Assert.IsNotNull(contentResult.Content);
        //    Assert.AreEqual(post, contentResult.Content.Data);
        //}

        //[TestMethod]
        //public void PostUser_ReturnErrorWhenNull()
        //{
        //    // Arrange
        //    var controller = new TesterController();

        //    // Act
        //    UserPOSTDTO post = null;
        //    var actionResult = controller.Post(post);
        //    var contentResult = actionResult as NotFoundResult;

        //    // Assert
        //    Assert.IsNotNull(contentResult);
        //    Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        //}
    }
}
