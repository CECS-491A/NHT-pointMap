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
            var valid_ssoID = Guid.NewGuid();
            var timestamp = 12312445;
            MockLoginPayload mock_payload = new MockLoginPayload
            {
                ssoUserId = valid_ssoID,
                email = valid_username,
                timestamp = timestamp
            };
            var endpoint = API_ROUTE_LOCAL + "/api/user/login";

            LoginDTO payload = new LoginDTO
            {
                Email = valid_username,
                SSOUserId = mock_payload.ssoUserId.ToString(),
                Timestamp = mock_payload.timestamp,
                Signature = mock_payload.Signature(),
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
            var existing_username = existing_user.Username;
            var existing_ssoID = existing_user.Id;
            var timestamp = 23454252;

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
            IHttpActionResult actionresult = controller.LoginFromSSO(payload);
            Assert.IsInstanceOfType(actionresult, typeof(OkNegotiatedContentResult<LoginResponseDTO>));
            var contentresult = actionresult as OkNegotiatedContentResult<LoginResponseDTO>;
            Assert.IsNotNull(contentresult);
        }
        [TestMethod]
        public void Logout_ExistingUser()
        {
            var controller = new UserController();
            var existing_user = ut.CreateUserObject();
            var session = ut.CreateSessionObject(existing_user);
            
            session = ut.CreateSessionInDb(session);
           

            var endpoint = API_ROUTE_LOCAL + "/#/login";

            LogoutDTO payload = new LogoutDTO
            {
                token = session.Token,
                Timestamp = DateTime.UtcNow.ToString()
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            IHttpActionResult actionresult = controller.LogoutFromSSO(payload);
            Assert.IsInstanceOfType(actionresult, typeof(OkNegotiatedContentResult<LogoutResponseDTO>));
            var contentresult = actionresult as OkNegotiatedContentResult<LogoutResponseDTO>;
            Assert.IsNotNull(contentresult);
        }
    }
}
