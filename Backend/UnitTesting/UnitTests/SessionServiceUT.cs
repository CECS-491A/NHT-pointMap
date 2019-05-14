﻿using System;
using System.Data.Entity.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using UnitTesting;


namespace Testing.UnitTests
{
    [TestClass]
    public class SessionServiceUT
    {
        DatabaseContext _db;
        SessionService ss;
        User newUser;
        Session newSession;
        TestingUtils tu;
        public SessionServiceUT()
        {
            //Arrange
            tu = new TestingUtils();
            _db = tu.CreateDataBaseContext();
            ss = new SessionService(_db);
        }

        [TestMethod]
        public void Create_Session_Success()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);
            var expected = newSession;
            using (_db = tu.CreateDataBaseContext())
            {
                // Act
                var response = ss.CreateSession(newSession);
                _db.SaveChanges();

                //Assert
                Assert.IsNotNull(response);
                Assert.AreEqual(response.Id, expected.Id);
            }
        }

        [TestMethod]
        public void Create_Session_RetrieveNew_Success()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);
            var expected = newSession;

            // Act
            var response = ss.CreateSession(newSession);
            _db.SaveChanges();

            //Assert
            var result = _db.Sessions.Find(newSession.Id);
            Assert.IsNotNull(response);
            Assert.IsNotNull(result);
            Assert.AreSame(result, expected);
        }

        [TestMethod]
        public void Create_Session_Fail_ExceptionThrown()
        {
            newUser = tu.CreateUserObject();

            // Arrange
            newSession = new Session
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(Session.MINUTES_UNTIL_EXPIRATION),
                CreatedAt = DateTime.UtcNow,
                User = newUser,
                UserId = newUser.Id

                //missing required fields
            };
            var expected = newSession;

            using (_db = tu.CreateDataBaseContext())
            {
                // ACT
                var response = ss.CreateSession(newSession);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    //catch error
                    // detach Session attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Sessions.Find(newSession.Id);

                // Assert
                Assert.IsNull(result);
                Assert.IsNotNull(response);
                Assert.AreEqual(expected, response);
                Assert.AreNotEqual(expected, result);
            }
        }

        [TestMethod]
        public void Delete_Session_Success()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);

            // Act
            newSession = ss.CreateSession(newSession);
            var expectedResponse = newSession;

            _db.SaveChanges();

            var response = ss.DeleteSession(newSession.Token);
            _db.SaveChanges();
            var result = _db.Sessions.Find(expectedResponse.Id);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Delete_Session_NonExisting()
        {
            // Arrange
            string nonExistingToken = Guid.NewGuid().ToString();

            using (_db = new DatabaseContext())
            {
                // Act
                var response = ss.DeleteSession(nonExistingToken);
                // will return null if Session does not exist
                _db.SaveChanges();
                var result = _db.Sessions.Find(response);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void Expire_Session_Success()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);

            // Act
            newSession = ss.CreateSession(newSession);
            var expectedResponse = newSession;

            _db.SaveChanges();

            var response = ss.ExpireSession(newSession.Token);
            _db.SaveChanges();
            var result = _db.Sessions.Find(expectedResponse.Id);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(result.ExpiresAt < DateTime.UtcNow);
            Assert.AreEqual(response.Id, expectedResponse.Id);
        }

        [TestMethod]
        public void Expire_Session_NonExisting()
        {
            // Arrange
            string nonExistingToken = Guid.NewGuid().ToString();

            using (_db = new DatabaseContext())
            {
                // Act
                var response = ss.ExpireSession(nonExistingToken);
                // will return null if Session does not exist
                _db.SaveChanges();
                var result = _db.Sessions.Find(response);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void Update_Session_Success()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);
            var expectedResultTime = newSession.CreatedAt;
            var expectedResult = newSession;

            // ACT
            newSession = ss.CreateSession(newSession);
            _db.SaveChanges();
            newSession.CreatedAt = newSession.CreatedAt.AddYears(60);
            var response = ss.UpdateSession(newSession);
            _db.SaveChanges();
            var result = _db.Sessions.Find(expectedResult.Id);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, expectedResult.Id);
            Assert.AreEqual(result.UpdatedAt, expectedResult.UpdatedAt);
            Assert.AreEqual(result.ExpiresAt, expectedResult.ExpiresAt);
            Assert.AreNotEqual(result.CreatedAt, expectedResultTime);
        }

        [TestMethod]
        public void Update_Session_NonExisting()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);
            Session expectedResult = newSession;

            // ACT
            var response = ss.UpdateSession(newSession);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception)
            {
                // catch error
                // rollback changes
                _db.Entry(newSession).State = System.Data.Entity.EntityState.Detached;
            }
            var result = _db.Sessions.Find(expectedResult.Id);

            // Assert
            Assert.IsNull(response);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Validate_Session_Success()
        {
            // Arrange 
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);
            var expectedResult = newSession;

            // ACT
            newSession = ss.CreateSession(newSession);
            _db.SaveChanges();
            var result = ss.ValidateSession(newSession.Token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult.Id, result.Id);
        }

        [TestMethod]
        public void Validate_Session_NonExisting()
        {
            // Arrange 
            newUser = tu.CreateUserObject();
            newSession = tu.CreateSessionObject(newUser);
            newSession.ExpiresAt = DateTime.UtcNow.AddMinutes(-45);
            var expectedResult = newSession;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                newSession = ss.CreateSession(newSession);
                _db.SaveChanges();
                var result = ss.ValidateSession(newSession.Token);

                // Assert
                Assert.IsNull(result);
            }
        }
    }
}
