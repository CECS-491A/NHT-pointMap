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
    public class UserControllerIT
    {
        string API_ROUTE_LOCAL = "http://localhost:58896";
        TestingUtils ut;

        public UserControllerIT()
        {
            ut = new TestingUtils();
        }

        [TestMethod]
        public void Login_NewUser_ValidUsername_200()
        {
            var controller = new UserController();
            var user = ut.CreateSSOUserInDb();
            var timestamp = 12312445;
            var expectedStatusCode = HttpStatusCode.Redirect;
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
            IHttpActionResult actionresult = controller.LoginFromSSO(payload);
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<string>));
            Assert.IsNotNull(actionresult as NegotiatedContentResult<string>);
            var result = actionresult as NegotiatedContentResult<string>;
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
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
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<string>));
            var contentresult = actionresult as NegotiatedContentResult<string>;
            Assert.IsNotNull(contentresult);
        }

        [TestMethod]
        public void LoginRegister_Invalid_Signature_401()
        {
            var controller = new UserController();
            var existing_user = ut.CreateSSOUserInDb();
            var existing_username = existing_user.Username;
            var existing_ssoID = existing_user.Id;
            var timestamp = 23454252;
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            MockLoginPayload mock_payload = new MockLoginPayload
            {
                ssoUserId = existing_ssoID,
                email = existing_username,
                timestamp = timestamp
            };

            // modify payload value
            var alterdEmail = "acces@mail.com";

            var endpoint = API_ROUTE_LOCAL + "/api/user/login";

            LoginDTO payload = new LoginDTO
            {
                Email = alterdEmail,
                SSOUserId = mock_payload.ssoUserId.ToString(),
                Timestamp = mock_payload.timestamp, 
                Signature = mock_payload.Signature(),
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            IHttpActionResult actionresult = controller.LoginFromSSO(payload);
            // returns a HTTPResponseMessage
            Assert.IsInstanceOfType(actionresult, typeof(ResponseMessageResult));
            var contentresult = actionresult as ResponseMessageResult;
            Assert.AreEqual(expectedStatusCode, contentresult.Response.StatusCode);
        }

        [TestMethod]
        public void Register_Attempt_InvalidEmail_400()
        {
            var controller = new UserController();
            var attemptedInvalidUsername = "notsstoredATdatabase.com";
            var attemptedSSOId = Guid.NewGuid();
            var attemptedTimestamp = 2345678;
            var expectedStatusCode = HttpStatusCode.BadRequest;

            MockLoginPayload mock_payload = new MockLoginPayload
            {
                ssoUserId = attemptedSSOId,
                email = attemptedInvalidUsername,
                timestamp = attemptedTimestamp
            };

            var endpoint = API_ROUTE_LOCAL + "/api/user/login";

            LoginDTO payload = new LoginDTO
            {
                Email = mock_payload.email,
                SSOUserId = mock_payload.ssoUserId.ToString(),
                Timestamp = mock_payload.timestamp,
                Signature = mock_payload.Signature(),
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            IHttpActionResult actionresult = controller.LoginFromSSO(payload);
            // returns a HTTPResponseMessage
            Assert.IsInstanceOfType(actionresult, typeof(ResponseMessageResult));
            var contentresult = actionresult as ResponseMessageResult;
            Assert.AreEqual(expectedStatusCode, contentresult.Response.StatusCode);
        }

        [TestMethod]
        public void Register_Attempt_InvalidSSOID_400()
        {
            var controller = new UserController();
            var attemptedUsername = "notstored@database.com";
            var attemptedSSOId = Guid.NewGuid();
            var attemptedTimestamp = 2345678;
            var expectedStatusCode = HttpStatusCode.BadRequest;

            MockLoginPayload mock_payload = new MockLoginPayload
            {
                ssoUserId = attemptedSSOId,
                email = attemptedUsername,
                timestamp = attemptedTimestamp
            };

            var endpoint = API_ROUTE_LOCAL + "/api/user/login";

            var makeAttemptedSSOIdInvalid = mock_payload.ssoUserId.ToString() + "838fjf57h2dhdn2dn";

            LoginDTO payload = new LoginDTO
            {
                Email = mock_payload.email,
                SSOUserId = makeAttemptedSSOIdInvalid,
                Timestamp = mock_payload.timestamp,
                Signature = mock_payload.Signature(),
            };

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            IHttpActionResult actionresult = controller.LoginFromSSO(payload);
            // returns a HTTPResponseMessage
            Assert.IsInstanceOfType(actionresult, typeof(ResponseMessageResult));
            var contentresult = actionresult as ResponseMessageResult;
            Assert.AreEqual(expectedStatusCode, contentresult.Response.StatusCode);
        }
    }
}
