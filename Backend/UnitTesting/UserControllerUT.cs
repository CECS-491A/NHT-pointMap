using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApi_PointMap.Controllers;
using WebApi_PointMap.Models;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using static UnitTesting.TestingUtils;
using System.Net;

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
            var user = ut.CreateSSOUserInDb();
            var timestamp = 12312445;
            var expectedStatusCode = HttpStatusCode.MovedPermanently;

            MockLoginPayload mock_payload = new MockLoginPayload
            {
                ssoUserId = user.Id,
                email = user.Username,
                timestamp = timestamp
            };
            var endpoint = API_ROUTE_LOCAL + "/api/user/login";

            LoginDTO payload = new LoginDTO
            {
                Email = user.Username,
                SSOUserId = mock_payload.ssoUserId.ToString(),
                Timestamp = mock_payload.timestamp,
                Signature = mock_payload.Signature(),
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            var actionresult = controller.LoginFromSSO(payload);
            Assert.IsInstanceOfType(actionresult, typeof(HttpResponseMessage));
            var contentresult = actionresult as HttpResponseMessage;
            Assert.AreEqual(expectedStatusCode, contentresult.StatusCode);
        }

        [TestMethod]
        public void Login_ExistingUser_ValidSSOID_200()
        {
            var controller = new UserController();
            var existing_user = ut.CreateSSOUserInDb();
            var existing_username = existing_user.Username;
            var existing_ssoID = existing_user.Id;
            var timestamp = 23454252;
            var expectedStatusCode = HttpStatusCode.MovedPermanently;

            MockLoginPayload mock_payload = new MockLoginPayload
            {
                ssoUserId = existing_ssoID,
                email = existing_username,
                timestamp = timestamp
            };

            var endpoint = API_ROUTE_LOCAL + "/api/user/login";

            LoginDTO payload = new LoginDTO
            {
                Email = existing_username,
                SSOUserId = mock_payload.ssoUserId.ToString(),
                Timestamp = mock_payload.timestamp,
                Signature = mock_payload.Signature(),
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            var actionresult = controller.LoginFromSSO(payload);
            Assert.IsInstanceOfType(actionresult, typeof(HttpResponseMessage));
            var contentresult = actionresult as HttpResponseMessage;
            Assert.AreEqual(expectedStatusCode, contentresult.StatusCode);
            Assert.IsNotNull(contentresult);
        }
    }
}
