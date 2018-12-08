using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.UserManagement
{
    public class UserManagementManager
    {
        private IPasswordService _passwordService;
        private IUserService _userService;

        public int CreateUser(string email, string password, DateTime dob)
        {
            DateTime timestamp = DateTime.UtcNow;
            _passwordService = new PasswordService();
            _userService = new UserService();
            byte[] salt = _passwordService.GenerateSalt();
            string hash = _passwordService.HashPassword(password, salt);
            User user = new User
            {
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                DateOfBirth = dob,
                UpdatedAt = timestamp
            };

            User responseUserService = null;
            int responseDB = 0;
            using (var _db = new DatabaseContext())
            {
                responseUserService = _userService.CreateUser(user, _db);
                responseDB = _db.SaveChanges();
            }
            return responseDB;
        }

        public int DeleteUser(User user)
        {
            _userService = new UserService();
            int response = 0;
            User responseObject = null;
            using (var _db = new DatabaseContext())
            {
                responseObject = _userService.DeleteUser(user.Id, _db);
                response = _db.SaveChanges();
            }
            return response;
        }

        public int DeleteUser(Guid id)
        {
            _userService = new UserService();
            int response = 0;
            User responseObject = null;
            using (var _db = new DatabaseContext())
            {
                responseObject = _userService.DeleteUser(id, _db);
                response = _db.SaveChanges();
            }
            return response;
        }

        public User GetUser(Guid id)
        {
            _userService = new UserService();
            User responseObject = null;
            using (var _db = new DatabaseContext())
            {
                responseObject = _userService.GetUser(id, _db);
            }
            return responseObject;
        }

        public User GetUser(string email)
        {
            _userService = new UserService();
            User responseObject = null;
            using (var _db = new DatabaseContext())
            {
                responseObject = _userService.GetUser(email, _db);
            }
            return responseObject;
        }

        public int DisableUser(User user)
        {
            User responseObject = null;
            int response;
            using (var _db = new DatabaseContext())
            {
                user.Disabled = true;
                responseObject = _userService.UpdateUser(user, _db);
                response = _db.SaveChanges();
            }
            return response;
        }

        public int EnableUser(User user)
        {
            User responseObject = null;
            int response;
            using (var _db = new DatabaseContext())
            {
                user.Disabled = false;
                responseObject = _userService.UpdateUser(user, _db);
                response = _db.SaveChanges();
            }
            return response;
        }

        public int UpdateUser(User user)
        {
            User responseObject = null;
            int response;
            using (var _db = new DatabaseContext())
            {
                responseObject = _userService.UpdateUser(user, _db);
                response = _db.SaveChanges();
            }
            return response;
        }
    }
}
