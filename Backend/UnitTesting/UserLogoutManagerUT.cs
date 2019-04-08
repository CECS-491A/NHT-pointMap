using System;
using System.Data.Entity.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using ManagerLayer.Login_Logout;
using ManagerLayer.AccessControl;

namespace UnitTesting
{
    [TestClass]
    public class UserLogoutManagerUT
    {
        DatabaseContext _db;
        SessionService ss;
        TestingUtils tu;
        UserLogoutManager lgtmgr;
        AuthorizationManager _authManager;

        public UserLogoutManagerUT()
        {
            _db = new DatabaseContext();
            tu = new TestingUtils();
            ss = new SessionService();
            lgtmgr = new UserLogoutManager();

        }

        [TestMethod]
        public void Logout_User_Existing_Session()
        {
            //Set up
            User user = tu.CreateUserObject();
            Session newSession = tu.CreateSessionObject(user);
            Session dummysession = tu.CreateSessionObject(user);
            newSession =tu.CreateSessionInDb(newSession);
            string sessionToken = newSession.Token;

         
            using (var _db = tu.CreateDataBaseContext())
            {

                //Act
                var response = lgtmgr.LogoutFromSSO(_db, newSession.Token);

                //Assert
                Assert.IsNotNull(response);
                Assert.AreEqual(response.Token, sessionToken);
                Assert.AreNotEqual(dummysession.Token, response.Token);
            }

        }

        [TestMethod]
        public void Logout_User_NonExisting_Session()
        {
            User user = tu.CreateUserObject();
            Session newSession = tu.CreateSessionObject(user);
            newSession = tu.CreateSessionInDb(newSession);
            Session newSessionFake = tu.CreateSessionObject(user);



            using (var _db = tu.CreateDataBaseContext())
            {
                var response = lgtmgr.LogoutFromSSO(_db, newSessionFake.Token);
                Assert.AreNotEqual(response.Token, newSessionFake.Token );
                
            }

        }
    }

}
