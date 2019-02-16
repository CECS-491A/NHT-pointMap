using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;

namespace UnitTesting
{
    [TestClass]
    public class SessionServiceUT
    {
        DatabaseContext _db;
        SessionService ss;
        UserService us;
        User u1;
        User u2;
        Session s1;
        Session s2;
        Session s3;
        TestingUtils tu;
        public SessionServiceUT()
        {
            //Arrange
            _db = new DatabaseContext();
            tu = new TestingUtils();
            ss = new SessionService();
            us = new UserService();
        }

        [TestMethod]
        public void GenerateSession_ValidUser_Pass()
        {
            //Arrange
            u1 = tu.CreateUserObject();
            u2 = tu.CreateUserObject();
            u1 = tu.CreateUserInDb(u1);
            u2 = tu.CreateUserInDb(u2);
            s1 = tu.CreateSession(u1);
            s2 = tu.CreateSession(u2);

            //Act
            Session result1 = tu.GetSession(u1);
            Session result2 = tu.GetSession(u2);

            //Assert
            Assert.AreEqual(s1, result1);
            Assert.AreEqual(s2, result2);
        }

        /*
        [TestMethod]
        public void validateSession()
        {
            //Assert.AreEqual(true, ss.ValidateSession(u1));
            //Assert.AreEqual(false, ss.ValidateSession(u2));
        }
        */
    }
}
