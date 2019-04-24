using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using WebApi_PointMap.Controllers;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace UnitTesting.IntegrationTests
{
    /// <summary>
    /// Summary description for UserManagementControllerIT
    /// </summary>
    [TestClass]
    public class UserManagementControllerIT
    {
        string API_Route_Local = "http://localhost:58896";
        TestingUtils _ut;

        public UserManagementControllerIT()
        {
            _ut = new TestingUtils();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>

        [TestMethod]
        public void GetAllUsers_Authorized_200()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            //_ut.CreateUserInDb(admin);
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            var expectedStatusCode = HttpStatusCode.OK;
            var expectedResultDataItem = new {

            };

            var endpoint = API_Route_Local + "/users";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            IHttpActionResult actionresult = controller.GetAllUsers();
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<string>));
        }
    }
}
