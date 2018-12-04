using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.UserManagers.Management
{
    public class UserManagementManager
    {
        private IPasswordService _passwordService;
        private IUserService _userService;

        public void CreateUser(string email, string password, DateTime dob)
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
            _userService.CreateUser(user);
        }

        public void DeleteUser(User user)
        {
            _userService = new UserService();
            _userService.DeleteUser(user);
        }

        public void DeleteUser(Guid id)
        {
            _userService = new UserService();
            _userService.DeleteUserById(id);
        }

        public User GetUser(Guid id)
        {
            _userService = new UserService();
            var user = _userService.GetUserById(id);
            return user;
        }

        public User GetUser(string email)
        {
            _userService = new UserService();
            var user = _userService.GetUserByEmail(email);
            return user;
        }

        public void DisableUser(User user)
        {
            // using autho
            user.Disabled = true;
            _userService.UpdateUser(user);
        }

        public void EnableUser(User user)
        {
            // using autho
            user.Disabled = false;
            _userService.UpdateUser(user);
        }

        public void UpdateUser(User user)
        {
            // user authoriations for updates
            _userService.UpdateUser(user);
        }
    }
}
