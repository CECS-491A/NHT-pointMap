using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApi_PointMap.Controllers;
using WebApi_PointMap.Models;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace UnitTesting
{
    /// <summary>
    /// Summary description for UserControllerUT
    /// </summary>
    [TestClass]
    public class UserControllerUT
    {
        string API_ROUTE_LOCAL = "http://localhost:58896";
        TestingUtils ut;

        public UserControllerUT()
        {
            ut = new TestingUtils();
        }

        [TestMethod]
        public void Login_NewUser_ValidUsername_200()
        {
            var controller = new UserController();
            var valid_username = Guid.NewGuid() + "@mail.com";
            var valid_ssoID = Guid.NewGuid().ToString();
            var valid_name = Guid.NewGuid().ToString();
            var endpoint = API_ROUTE_LOCAL + "/api/sso/user/login";

            LoginDTO payload = new LoginDTO
            {
                Email = valid_username,
                SSOUserId = valid_ssoID,
                Name = valid_name
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            IHttpActionResult actionresult = controller.LoginFromSSO(payload);
            Assert.IsInstanceOfType(actionresult, typeof(OkNegotiatedContentResult<LoginResponseDTO>));
            Assert.IsNotNull(actionresult as OkNegotiatedContentResult<LoginResponseDTO>);
        }

        [TestMethod]
        public void Login_ExistingUser_ValidSSOID_200()
        {
            var controller = new UserController();
            var existing_user = ut.CreateSSOUserInDb();
            var existing_username = existing_user.Email;
            var existing_ssoID = existing_user.SSOId;
            var existing_name = Guid.NewGuid().ToString();
           
            var endpoint = API_ROUTE_LOCAL + "/api/sso/user/login";

            LoginDTO payload = new LoginDTO
            {
                Email = existing_username,
                SSOUserId = existing_ssoID.ToString(),
                Name = existing_name
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            IHttpActionResult actionresult = controller.LoginFromSSO(payload);
            Assert.IsInstanceOfType(actionresult, typeof(OkNegotiatedContentResult<LoginResponseDTO>));
            var contentresult = actionresult as OkNegotiatedContentResult<LoginResponseDTO>;
            Assert.IsNotNull(contentresult);
            Assert.AreEqual(existing_user.Id, contentresult.Content.userId);
        }
    }
}
