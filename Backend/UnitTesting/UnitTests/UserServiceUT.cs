using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.UserManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;
using static ServiceLayer.Services.ExceptionService;
using UnitTesting;

namespace Testing.UnitTests
{
    [TestClass]
    public class UserServiceUT
    {
        User newUser;
        TestingUtils tu;
        UserService us;
        DatabaseContext _db;
        UserManagementManager _umm;

        public UserServiceUT()
        {
            tu = new TestingUtils();
        }

        [TestMethod]
        public void Create_User_Success()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            var expected = newUser;
            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);

                // Act
                var response = us.CreateUser(newUser);
                _db.SaveChanges();

                //Assert
                Assert.IsNotNull(response);
                Assert.AreSame(response, expected);
            }
        }

        [TestMethod]
        public void Create_User_RetrieveNew_Success()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            var expected = newUser;

            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);

                // Act
                User response = us.CreateUser(newUser);
                _db.SaveChanges();

                //Assert
                var result = _db.Users.Find(newUser.Id);
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreSame(result, expected);
            }
        }

        [TestMethod]
        public void Create_User_Fail_ExceptionThrown()
        {
            // Arrange
            newUser = new User
            {
                Username = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                
                // missing required fields
            };
            var expected = newUser;

            using (_db = tu.CreateDataBaseContext())
            {
                // ACT
                us = new UserService(_db);
                var response = us.CreateUser(newUser);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    //catch error
                    // detach user attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Users.Find(newUser.Id);

                // Assert
                Assert.IsNull(result);
                Assert.IsNotNull(response);
                Assert.AreEqual(expected, response);
                Assert.AreNotEqual(expected, result);
            }
        }

        [TestMethod]
        public void Create_User_Using_Manager()
        {
            // Arrange
            string Username = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com";
            string password = (Guid.NewGuid()).ToString();
            DateTime dob = DateTime.UtcNow;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                _umm = new UserManagementManager(_db);
                var response = _umm.CreateUser(Username, Guid.NewGuid());
                _db.SaveChanges();
                var result = _umm.GetUser(response.Id);

                // Assert 
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreEqual(Username, result.Username);
            } 
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEmailException))]
        public void Create_User_Using_Manager_NotRealEmail()
        {
            // Arrange
            string email = Guid.NewGuid() + ".com";
            string password = (Guid.NewGuid()).ToString();
            DateTime dob = DateTime.UtcNow;

            // Act
            using (var _db = tu.CreateDataBaseContext())
            {
                _umm = new UserManagementManager(_db);
                var response = _umm.CreateUser(email, Guid.NewGuid());

                // Assert 
                //expects exception
            }
           
        }

        [TestMethod]
        public void Delete_User_Success()
        {
            // Arrange
            newUser = tu.CreateUserInDb();

            var expectedResponse = newUser;

            using (_db = tu.CreateDataBaseContext())
            {
                // Act
                us = new UserService(_db);
                var response = us.DeleteUser(newUser.Id);
                _db.SaveChanges();
                var result = _db.Users.Find(expectedResponse.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
                Assert.AreEqual(response.Id, expectedResponse.Id);
            }
        }

        [TestMethod]
        public void Delete_User_NonExisting()
        {
            // Arrange
            Guid nonExistingId = Guid.NewGuid();

            var expectedResponse = nonExistingId;

            using (_db = new DatabaseContext())
            {
                us = new UserService(_db);
                // Act
                var response = us.DeleteUser(nonExistingId);
                // will return null if user does not exist
                _db.SaveChanges();
                var result = _db.Users.Find(expectedResponse);

                // Assert
                Assert.IsNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void Update_User_Success()
        {
            // Arrange
            newUser = tu.CreateUserInDb();
            newUser.City = "Long Beach";
            var expectedResponse = newUser;
            var expectedResult = newUser;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);

                var response = us.UpdateUser(newUser);
                _db.SaveChanges();
                var result = _db.Users.Find(expectedResult.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Id, expectedResult.Id);
                Assert.AreEqual(result.City, expectedResult.City);
            }

        }

        [TestMethod]
        public void Update_User_NonExisting_why()
        {
            // Arrange
            newUser = tu.CreateUserObject();
            newUser.City = "Long Beach";
            var expectedResponse = newUser;
            var expectedResult = newUser;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);

                var response = us.UpdateUser( newUser);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    // catch error
                    // rollback changes
                    _db.Entry(newUser).State = System.Data.Entity.EntityState.Detached;
                }
                var result = _db.Users.Find(expectedResult.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void Update_User_OnRequiredValue()
        {
            // Arrange
            newUser = tu.CreateUserInDb();
            var expectedResult = newUser;
            newUser.PasswordHash = null;
            var expectedResponse = newUser;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);
                var response = us.UpdateUser(newUser);
                try
                {
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException)
                {
                    // catch error
                    // rollback changes
                    _db.Entry(response).CurrentValues.SetValues(_db.Entry(response).OriginalValues);
                    _db.Entry(response).State = System.Data.Entity.EntityState.Unchanged;
                }
                var result = _db.Users.Find(expectedResult.Id);

                // Assert
                Assert.IsNotNull(response);
                Assert.AreEqual(expectedResponse, response);
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult, result);
            }
        }

        [TestMethod]
        public void Get_User_Success()
        {
            // Arrange 

            newUser = tu.CreateUserInDb();
            var expectedResult = newUser;

            // ACT
            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);

                var result = us.GetUser(expectedResult.Id);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult.Id, result.Id);
            }
        }

        [TestMethod]
        public void Get_User_NonExisting()
        {
            // Arrange
            Guid nonExistingUser = Guid.NewGuid();
            User expectedResult = null;

            // Act
            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);

                var result = us.GetUser(nonExistingUser);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(expectedResult, result);
            }
        }

        [TestMethod]
        public void Disable_User_Success()
        {
            // Arrange
            newUser = tu.CreateUserInDb();
            var expectedResponse = newUser;
            var expectedResult = true;

            // ACT
            using (var _db = new DatabaseContext())
            {
                _umm = new UserManagementManager(_db);
                _umm.DisableUser(newUser);
                _db.SaveChanges();
                var result = _umm.GetUser(newUser.Id);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult, result.Disabled);
            }
           
        }

        [TestMethod]
        public void Enable_User_Success()
        {
            // Arrange
            newUser = tu.CreateUserInDb();
            var expectedResponse = newUser;
            var expectedResult = false;

            // ACT
            using (var _db = new DatabaseContext())
            {
                _umm = new UserManagementManager(_db);
                _umm.EnableUser(newUser);
                _db.SaveChanges();
                var result = _umm.GetUser(newUser.Id);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult, result.Disabled);
            }
        }

        [TestMethod]
        public void Toggle_User_Success()
        {
            // Arrange
            newUser = tu.CreateUserInDb();
            var expectedResult = !newUser.Disabled;

            // ACT
            using (var _db = new DatabaseContext())
            {
                _umm = new UserManagementManager(_db);
                _umm.ToggleUser(newUser, true);
                _db.SaveChanges();
                var result = _umm.GetUser(newUser.Id);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(expectedResult, result.Disabled);
            }
        }

        // Check that IsUserManager returns false when checking against an unassociated user
        [TestMethod]
        public void Is_User_Manager_Over_Independent()
        {
            User unassociatedUser = tu.CreateUserObject();
            tu.CreateUserInDb(unassociatedUser);

            User subject = tu.CreateUserObject();
            tu.CreateUserInDb(subject);

            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);
                Assert.IsFalse(us.IsManagerOver(unassociatedUser, subject));
            }
        }

        // Check that IsUserManager returns false when checking against a user in another branch of the tree
        [TestMethod]
        public void Is_User_Manager_Over_Different_Branch()
        {
            User unassociatedUser = tu.CreateUserInDb();

            User directManager = tu.CreateUserInDb();

            User subject = tu.CreateUserObject();
            subject.ManagerId = directManager.Id;
            subject = tu.CreateUserInDb(subject);

            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);
                Assert.IsFalse(us.IsManagerOver(unassociatedUser, subject));
            }
        }

        // Check that IsUserManager returns true when checking against a direct manager
        [TestMethod]
        public void Is_User_Manager_Over_Direct()
        {
            User directManager = tu.CreateUserInDb();

            User subject = tu.CreateUserObject();
            subject.ManagerId = directManager.Id;
            subject = tu.CreateUserInDb(subject);

            using (_db = tu.CreateDataBaseContext())
            {
                us = new UserService(_db);
                Assert.IsTrue(us.IsManagerOver(directManager, subject));
            }
        }

        // Check that IsUserManager returns true when checking against a indirect manager
        [TestMethod]
        public void Is_User_Manager_Over_Indirect()
        {
            User indirectManager = tu.CreateUserInDb();

            User directManager = tu.CreateUserObject();
            directManager.ManagerId = indirectManager.Id;
            directManager = tu.CreateUserInDb(directManager);

            User subject = tu.CreateUserObject();
            subject.ManagerId = directManager.Id;
            subject = tu.CreateUserInDb(subject);

            using (_db = tu.CreateDataBaseContext())
            {
                _db.SaveChanges();
                us = new UserService(_db);
                Assert.IsTrue(us.IsManagerOver(indirectManager, subject));
            }
        }

        [TestMethod]
        public void Get_All_Users_Success()
        {
            Type expectedType = typeof(IEnumerable<User>);

            using (var _db = new DatabaseContext())
            {
                us = new UserService(_db);

                var users = us.GetAllUsers();
                Assert.IsNotNull(users);
                Assert.IsInstanceOfType(users, expectedType);
            }
            
        }
    }
}
