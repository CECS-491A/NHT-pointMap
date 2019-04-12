using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApi_PointMap.Controllers;
using DataAccessLayer.Models;
using System.Net.Http;
using System.Web.Http;
using WebApi_PointMap.Models;
using ServiceLayer.Services;
using DataAccessLayer.Database;
using System.Web.Http.Results;
using System.Net;
using System.Threading;
using System.Diagnostics;
using DTO.DTO;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class ErrorHandlingIT
    {
        const string API_ROUTE_LOCAL = "http://localhost:58896";
        PointController _pointController;
        UserManagementController _umController;
        TestingUtils _tu;
        User newUser;
        SessionService _ss;
        DatabaseContext _db;

        public ErrorHandlingIT()
        {
            _pointController = new PointController();
            _umController = new UserManagementController();
            _tu = new TestingUtils();
            _ss = new SessionService();
        }

        [TestMethod]
        public void GetUsersUnderManager_InvalidManagerId_412()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/users/";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            IHttpActionResult response = _umController.GetUsersUnderManager(null);

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.PreconditionFailed, result.StatusCode);
            Assert.AreEqual("Invalid payload.", result.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void GetUsersUnderManager_InvalidManagerId_400()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);
            var badId = "q7h493proannaosnfdo";

            var endpoint = API_ROUTE_LOCAL + "/users/";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            IHttpActionResult response = _umController.GetUsersUnderManager(badId);

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid User Id.", result.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void DeleteUser_NoTokenProvided_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/delete";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();

            _umController.Request = request;

            IHttpActionResult response = _umController.DeleteUser(newUser.Id.ToString());

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.AreEqual("https://kfc-sso.com/#/login", result.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void DeleteUser_NoUserIdProvided_412()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/delete";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            IHttpActionResult response = _umController.DeleteUser((string)null);

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.PreconditionFailed, result.StatusCode);
            Assert.AreEqual("Invalid payload.", result.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void DeleteUser_SessionExpired_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            //_tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/delete";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            IHttpActionResult response = _umController.DeleteUser(newUser.Id.ToString());

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.AreEqual("https://kfc-sso.com/#/login", result.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void UpdateUser_InvalidUserId_404()
        {
            newUser = _tu.CreateUserObject();
            newUser.IsAdministrator = true;
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/update";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            UpdateUserRequestDTO userDTO = new UpdateUserRequestDTO
            {
                Id = Guid.NewGuid().ToString(),
                City = newUser.City,
                State = newUser.State,
                Country = newUser.Country,
                Manager = newUser.ManagerId.ToString(),
                IsAdmin = newUser.IsAdministrator,
                Disabled = newUser.Disabled
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            IHttpActionResult response = _umController.UpdateUser(userDTO);

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            Assert.AreEqual("User does not exist.", result.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void DeleteUserSSO_InvalidtokenSignature_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/sso/user/delete";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            LoginDTO loginDTO = new LoginDTO
            {
                SSOUserId = Guid.NewGuid().ToString(),
                Email = "something@email.com",
                Timestamp = 1928743091,
                Signature = "ahsbdkfhasjdfln",
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            IHttpActionResult response = _umController.DeleteUser(loginDTO);

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.AreEqual("https://kfc-sso.com/#/login", result.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void CreatePoint_InvalidLongLat_400()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/api/point";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            PointPOST point = new PointPOST
            {
                Longitude = 185,
                Latitude = 85,
                Description = "bad longitude value",
                Name = "test bad point",
                Id = Guid.NewGuid()
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            IHttpActionResult response = _pointController.Post(point);

            var result = response.ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Longitude/Latitude value invalid.", result.Content.ReadAsStringAsync().Result);
        }
    }
}
