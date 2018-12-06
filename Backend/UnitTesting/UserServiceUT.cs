using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

namespace UnitTesting
{
    [TestClass]
    public class UserServiceUT
    {
        User newUser;
        TestingUtils tu;
        UserService us;

        public UserServiceUT()
        {
            us = new UserService();
            tu = new TestingUtils();
            newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };
        }

        [TestMethod]
        public void Create_User_Success()
        {
            // Act
            User newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };
            int response = us.CreateUser(newUser);

            //Assert
            Assert.IsTrue(response > 0);
            if (response > 0)
                us.DeleteUser(newUser.Id);
        }

        [TestMethod]
        public void Create_User_Fail()
        {
            // Act
            User newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };
            int response = us.CreateUser(newUser);

            //Assert
            Assert.IsTrue(response < 1);
            if (response > 0)
                us.DeleteUser(newUser.Id);
        }

        [TestMethod]
        public void Delete_User_Success()
        {
            // ACT
            User newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };
            us.CreateUser(newUser);

            int response = us.DeleteUser(newUser.Id);

            // Assert
            Assert.IsTrue(response > 0);
        }

        [TestMethod]
        public void Delete_User_NonExisting()
        {
            // ACT
            int response = us.DeleteUser(Guid.NewGuid());

            // Assert
            Assert.IsTrue(response < 1);
        }

        [TestMethod]
        public void Update_User_Success()
        {
            User newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };
            us.CreateUser(newUser);

            // ACT
            newUser.City = "Long Beach";
            var copy = newUser;
            var reponse = us.UpdateUser(newUser);
            var responseUser = us.GetUser(newUser.Id);

            // Assert
            Assert.IsTrue(reponse > 0);
            Assert.AreEqual(copy.City, responseUser.City);
        }

        [TestMethod]
        public void Update_User_NonExisting()
        {
            User newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };

            // ACT
            newUser.UpdatedAt = DateTime.UtcNow;
            var copy = newUser;
            var response = us.UpdateUser(newUser);
            var responseUser = us.GetUser(newUser.Id);

            // Assert
            Assert.IsNull(responseUser);
            Assert.IsTrue(response < 1);
        }

        [TestMethod]
        public void Get_User_Success()
        {
            User newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };

            // ACT
            us.CreateUser(newUser);
            User response = us.GetUser(newUser.Id);

            // Assert
            Assert.AreEqual(response.Id, newUser.Id);
        }

        [TestMethod]
        public void Get_User_NonExisting()
        {
            // ACT
            User nonExistingUser = us.GetUser(Guid.NewGuid());

            // Assert
            Assert.IsNull(nonExistingUser);
        }

        // disabling and enabling a user same test as update user
        [TestMethod]
        public void Disable_User_Success()
        {
            User newUser = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = tu.GetRandomness()
            };
            us.CreateUser(newUser);

            // ACT
            newUser.Disabled = true;
            var copy = newUser;
            var reponse = us.UpdateUser(newUser);
            var responseUser = us.GetUser(newUser.Id);

            // Assert
            Assert.IsTrue(reponse > 0);
            Assert.AreEqual(copy.Disabled, responseUser.Disabled);
        }

    }
}
