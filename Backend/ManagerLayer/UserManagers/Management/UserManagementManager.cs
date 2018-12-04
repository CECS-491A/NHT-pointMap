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
            _userService.Create(user);
        }

        public void DeleteUser()
        {

        }

        public void GetUser()
        {

        }

        public void UpdateUser()
        {

        }
    }
}
