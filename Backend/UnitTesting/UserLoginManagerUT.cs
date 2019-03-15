using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ServiceLayer.Services.ExceptionService;
using ManagerLayer.Login;
using ManagerLayer.AccessControl;
using DataAccessLayer.Models;

namespace UnitTesting
{
    /// <summary>
    /// Summary description for LoginManagerUT
    /// </summary>
    [TestClass]
    public class UserLoginManagerUT
    {
        readonly TestingUtils ut;
        readonly UserLoginManager _loginManager;
        readonly AuthorizationManager _authManager;

        public UserLoginManagerUT()
        {
            ut = new TestingUtils();
            _loginManager = new UserLoginManager();
            _authManager = new AuthorizationManager();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEmailException))]
        public void Login_NewUser_InvalidUserName_Failure_ExceptionThrown()
        {
            var invalid_username = Guid.NewGuid() + ".com";
            var valid_ssoID = Guid.NewGuid();

            using (var _db = ut.CreateDataBaseContext())
            {
                _loginManager.LoginFromSSO(_db, invalid_username, valid_ssoID);
            }

            //Assert - catch exception
        }

        [TestMethod]
        public void Login_NewUser_ValidUserName_Success()
        {
            var valid_username = Guid.NewGuid() + "@mail.com";
            var valid_ssoID = Guid.NewGuid();

            using (var _db = ut.CreateDataBaseContext())
            {
                var response = _loginManager.LoginFromSSO(_db, valid_username, valid_ssoID);
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public void Login_ExistingUser_Success()
        {
            var existing_user = ut.CreateSSOUserInDb();
            var existing_username = existing_user.Email;
            var existing_ssoID = existing_user.SSOId;

            using (var _db = ut.CreateDataBaseContext())
            {
                var response = _loginManager.LoginFromSSO(_db, existing_username, existing_ssoID);
                Assert.IsNotNull(response);
                Assert.AreEqual(existing_user.Id, response.userid);
            }
        }
    }
}
