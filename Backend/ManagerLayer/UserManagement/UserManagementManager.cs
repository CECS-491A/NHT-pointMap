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
            return _userService.CreateUser(user);
        }

        public int DeleteUser(User user)
        {
            _userService = new UserService();
            return _userService.DeleteUser(user.Id);
        }

        public int DeleteUser(Guid id)
        {
            _userService = new UserService();
            return _userService.DeleteUser(id);
        }

        public User GetUser(Guid id)
        {
            _userService = new UserService();
            var user = _userService.GetUser(id);
            return user;
        }

        public User GetUser(string email)
        {
            _userService = new UserService();
            var user = _userService.GetUser(email);
            return user;
        }

        public int DisableUser(User user)
        {
            // using autho
            user.Disabled = true;
            return _userService.UpdateUser(user);
        }

        public int EnableUser(User user)
        {
            // using autho
            user.Disabled = false;
            return _userService.UpdateUser(user);
        }

        public int UpdateUser(User user)
        {
            // user authoriations for updates
            return _userService.UpdateUser(user);
        }
    }
}
