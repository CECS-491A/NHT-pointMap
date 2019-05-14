using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using WebApi_PointMap.Controllers;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using UnitTesting;
using ManagerLayer.UserManagement;
using DTO.UserManagementAPI;

namespace Testing.IntegrationTests
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

        [TestMethod]
        public void NoSessionToken_Unauthorized_401()
        {
            var controller = new UserManagementController();

            var expectedStatusCode = HttpStatusCode.Unauthorized;

            var endpoint = API_Route_Local + "/users";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            // no token header was added to request

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.GetAllUsers();
            Assert.AreEqual(expectedStatusCode,actionresult.StatusCode);
        }

        [TestMethod]
        public void FakeSessionToken_Unauthorized_401()
        {
            var controller = new UserManagementController();

            var expectedStatusCode = HttpStatusCode.Unauthorized;

            var endpoint = API_Route_Local + "/users";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            // fake token header added
            controller.Request.Headers.Add("token", "thisisafaketoken");

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.GetAllUsers();
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void GetAllUsers_Authorized_200()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            var expectedStatusCode = HttpStatusCode.OK;

            var endpoint = API_Route_Local + "/users";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            IHttpActionResult actionresult = controller.GetAllUsers();
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<List<GetAllUsersResponseDataItem>>));
            var contentresult = actionresult as NegotiatedContentResult<List<GetAllUsersResponseDataItem>>;
            Assert.AreEqual(expectedStatusCode, contentresult.StatusCode);
        }

        [TestMethod]
        public void GetAllUsers_Unauthorized_401()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = false;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            var expectedStatusCode = HttpStatusCode.Unauthorized;

            var endpoint = API_Route_Local + "/users";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.GetAllUsers();
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void GetUser_Authorized_200()
        {
            var controller = new UserManagementController();
            var user = _ut.CreateUserObject();
            var userSession = _ut.CreateSessionObject(user);
            _ut.CreateSessionInDb(userSession);

            var expectedStatusCode = HttpStatusCode.OK;

            var endpoint = API_Route_Local + "/user";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", userSession.Token);

            IHttpActionResult actionresult = controller.GetUser();
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<GetUserResponseData>));
            var contentresult = actionresult as NegotiatedContentResult<GetUserResponseData>;
            Assert.AreEqual(expectedStatusCode, contentresult.StatusCode);
        }

        [TestMethod]
        public void DeleteUser_Authorized_200()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);
            var userToDelete = _ut.CreateUserInDb().Id;

            var expectedStatusCode = HttpStatusCode.OK;

            var endpoint = API_Route_Local + "/user/delete/{userId}";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            IHttpActionResult actionresult = controller.DeleteUser(userToDelete.ToString());
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<string>));
            var contentresult = actionresult as NegotiatedContentResult<string>;
            Assert.AreEqual(expectedStatusCode, contentresult.StatusCode);

            // persistence test
            using (var _db = _ut.CreateDataBaseContext())
            {
                var getUser = _db.Users.Find(userToDelete);
                Assert.IsNull(getUser);
            }
        }

        [TestMethod]
        public void DeleteUser_Unauthorized_401()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = false;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);
            var userToDelete = _ut.CreateUserInDb().Id.ToString();

            var expectedStatusCode = HttpStatusCode.Unauthorized;

            var endpoint = API_Route_Local + "/user/delete/{userId}";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.DeleteUser(userToDelete);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void DeleteNonExistingUser_Authorized_404()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);
            var nonexistingUserToDelete = Guid.NewGuid().ToString();

            var expectedStatusCode = HttpStatusCode.NotFound;

            var endpoint = API_Route_Local + "/user/delete/{userId}";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.DeleteUser(nonexistingUserToDelete);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void UpdateUser_Authorized_200()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);
            var existingUser = _ut.CreateUserInDb();

            // modify user 
            var modifiedUser = _ut.CreateUserObject();
            modifiedUser.Id = existingUser.Id;
            modifiedUser.IsAdministrator = existingUser.IsAdministrator;
            modifiedUser.Disabled = true;
            modifiedUser.City = "Long Beach";

            // mock payload
            var mock_payload = new UpdateUserRequestDTO
            {
                Id = modifiedUser.Id.ToString(),
                City = modifiedUser.City,
                State = modifiedUser.State,
                Country = modifiedUser.Country,
                Manager = modifiedUser.ManagerId.ToString(),
                IsAdmin = modifiedUser.IsAdministrator,
                Disabled = modifiedUser.Disabled
            };

            var expectedStatusCode = HttpStatusCode.OK;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            IHttpActionResult actionresult = controller.UpdateUser(mock_payload);
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<string>));
            var contentresult = actionresult as NegotiatedContentResult<string>;
            Assert.AreEqual(expectedStatusCode, contentresult.StatusCode);
            
            // persistence test
            using (var _db = _ut.CreateDataBaseContext())
            {
                var getUser = _db.Users.Find(modifiedUser.Id);
                Assert.AreNotEqual(existingUser, getUser);
                Assert.AreEqual(existingUser.Id, getUser.Id);
                Assert.AreEqual(existingUser.IsAdministrator, getUser.IsAdministrator);
            }
        }

        [TestMethod]
        public void UpdateUser_Unauthorized_401()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = false;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);
            var existingUser = _ut.CreateUserInDb();

            // modify user 
            var modifiedUser = _ut.CreateUserObject();
            modifiedUser.Id = existingUser.Id;
            modifiedUser.IsAdministrator = existingUser.IsAdministrator;
            modifiedUser.Disabled = true;
            modifiedUser.City = "Long Beach";

            // mock payload
            var mock_payload = new UpdateUserRequestDTO
            {
                Id = modifiedUser.Id.ToString(),
                City = modifiedUser.City,
                State = modifiedUser.State,
                Country = modifiedUser.Country,
                Manager = modifiedUser.ManagerId.ToString(),
                IsAdmin = modifiedUser.IsAdministrator,
                Disabled = modifiedUser.Disabled
            };

            var expectedStatusCode = HttpStatusCode.Unauthorized;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.UpdateUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void UpdateNonExistingUser_Authorized_404()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            var nonexistingUserId = Guid.NewGuid();

            // modify user 
            var modifiedUser = _ut.CreateUserObject();
            modifiedUser.Id = nonexistingUserId;

            // mock payload
            var mock_payload = new UpdateUserRequestDTO
            {
                Id = modifiedUser.Id.ToString(),
                City = modifiedUser.City,
                State = modifiedUser.State,
                Country = modifiedUser.Country,
                Manager = modifiedUser.ManagerId.ToString(),
                IsAdmin = modifiedUser.IsAdministrator,
                Disabled = modifiedUser.Disabled
            };

            var expectedStatusCode = HttpStatusCode.NotFound;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.UpdateUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

      

        [TestMethod]
        public void UpdateUser_NonexistingManager_404()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);
            var existingUser = _ut.CreateUserInDb();
            var nonexistingManagerId = Guid.NewGuid();

            // modify user 
            var modifiedUser = _ut.CreateUserObject();
            modifiedUser.Id = existingUser.Id;
            modifiedUser.IsAdministrator = existingUser.IsAdministrator;
            modifiedUser.ManagerId = nonexistingManagerId;

            // mock payload
            var mock_payload = new UpdateUserRequestDTO
            {
                Id = modifiedUser.Id.ToString(),
                City = modifiedUser.City,
                State = modifiedUser.State,
                Country = modifiedUser.Country,
                Manager = modifiedUser.ManagerId.ToString(),
                IsAdmin = modifiedUser.IsAdministrator,
                Disabled = modifiedUser.Disabled
            };

            var expectedStatusCode = HttpStatusCode.NotFound;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.UpdateUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void UpdateUser_InvalidManagerGuid_400()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);
            var existingUser = _ut.CreateUserInDb();
            var invalidManagerId = Guid.NewGuid().ToString() + "makeinvalid";

            // modify user 
            var modifiedUser = _ut.CreateUserObject();
            modifiedUser.Id = existingUser.Id;
            modifiedUser.IsAdministrator = existingUser.IsAdministrator;

            // mock payload
            var mock_payload = new UpdateUserRequestDTO
            {
                Id = modifiedUser.Id.ToString(),
                City = modifiedUser.City,
                State = modifiedUser.State,
                Country = modifiedUser.Country,
                Manager = invalidManagerId,
                IsAdmin = modifiedUser.IsAdministrator,
                Disabled = modifiedUser.Disabled
            };

            var expectedStatusCode = HttpStatusCode.BadRequest;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.UpdateUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void CreateNewUser_Authorized_201()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            // modify user 
            var newUser = new {
                username = Guid.NewGuid() + "@mail.com",
                city = "Long Beach",
                state = "California",
                country = "USA",
                manager = "",
                isadmin = false,
                disabled = false,
            };

            // mock payload
            var mock_payload = new CreateUserRequestDTO
            {
                Username = newUser.username,
                City = newUser.city,
                State = newUser.state,
                Country = newUser.country,
                Manager = newUser.manager,
                IsAdmin = newUser.isadmin,
                Disabled = newUser.disabled
            };

            var expectedStatusCode = HttpStatusCode.Created;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            IHttpActionResult actionresult = controller.CreateNewUser(mock_payload);
            Assert.IsInstanceOfType(actionresult, typeof(NegotiatedContentResult<string>));
            var contentresult = actionresult as NegotiatedContentResult<string>;
            Assert.AreEqual(expectedStatusCode, contentresult.StatusCode);

            // persistence test
            using (var _db = _ut.CreateDataBaseContext())
            {
                var _userManager = new UserManagementManager(_db);
                var getUser = _userManager.GetUser(newUser.username);
                Assert.AreNotEqual(newUser, getUser);
                Assert.AreEqual(newUser.isadmin, getUser.IsAdministrator);
            }
        }

        [TestMethod]
        public void CreateNewUser_Unauthorized_401()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = false;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            // modify user 
            var newUser = new
            {
                username = Guid.NewGuid() + "@mail.com",
                city = "Long Beach",
                state = "California",
                country = "USA",
                manager = "",
                isadmin = false,
                disabled = false,
            };

            // mock payload
            var mock_payload = new CreateUserRequestDTO
            {
                Username = newUser.username,
                City = newUser.city,
                State = newUser.state,
                Country = newUser.country,
                Manager = newUser.manager,
                IsAdmin = newUser.isadmin,
                Disabled = newUser.disabled
            };

            var expectedStatusCode = HttpStatusCode.Unauthorized;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.CreateNewUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void CreateNewUser_InvalidUsername_400()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            var invalidUsername = Guid.NewGuid() + "ATmail.com";

            // modify user 
            var newUser = new
            {
                username = invalidUsername,
                city = "Long Beach",
                state = "California",
                country = "USA",
                manager = "",
                isadmin = false,
                disabled = false,
            };

            // mock payload
            var mock_payload = new CreateUserRequestDTO
            {
                Username = newUser.username,
                City = newUser.city,
                State = newUser.state,
                Country = newUser.country,
                Manager = newUser.manager,
                IsAdmin = newUser.isadmin,
                Disabled = newUser.disabled
            };

            var expectedStatusCode = HttpStatusCode.BadRequest;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.CreateNewUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void CreateNewUser_InvalidManagerGuid_400()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            var invalidManagerId = Guid.NewGuid() + "invalidid";

            // modify user 
            var newUser = new
            {
                username = Guid.NewGuid() + "@mail.com",
                city = "Long Beach",
                state = "California",
                country = "USA",
                manager = invalidManagerId,
                isadmin = false,
                disabled = false,
            };

            // mock payload
            var mock_payload = new CreateUserRequestDTO
            {
                Username = newUser.username,
                City = newUser.city,
                State = newUser.state,
                Country = newUser.country,
                Manager = newUser.manager,
                IsAdmin = newUser.isadmin,
                Disabled = newUser.disabled
            };

            var expectedStatusCode = HttpStatusCode.BadRequest;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.CreateNewUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }

        [TestMethod]
        public void CreateNewUser_NonexistingManager_404()
        {
            var controller = new UserManagementController();
            var admin = _ut.CreateUserObject();
            admin.IsAdministrator = true;
            var adminSession = _ut.CreateSessionObject(admin);
            _ut.CreateSessionInDb(adminSession);

            var nonexistingdManagerId = Guid.NewGuid();

            // modify user 
            var newUser = new
            {
                username = Guid.NewGuid() + "@mail.com",
                city = "Long Beach",
                state = "California",
                country = "USA",
                manager = nonexistingdManagerId.ToString(),
                isadmin = false,
                disabled = false,
            };

            // mock payload
            var mock_payload = new CreateUserRequestDTO
            {
                Username = newUser.username,
                City = newUser.city,
                State = newUser.state,
                Country = newUser.country,
                Manager = newUser.manager,
                IsAdmin = newUser.isadmin,
                Disabled = newUser.disabled
            };

            var expectedStatusCode = HttpStatusCode.NotFound;

            var endpoint = API_Route_Local + "/user/update";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };
            controller.Request.Headers.Add("token", adminSession.Token);

            NegotiatedContentResult<string> actionresult = (NegotiatedContentResult<string>)controller.CreateNewUser(mock_payload);
            Assert.AreEqual(expectedStatusCode, actionresult.StatusCode);
        }
    }
}
