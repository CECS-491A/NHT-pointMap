using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.UserManagement
{
    public class UserManagementManager
    {
        DatabaseContext _db;
        UserService _userService;

        public UserManagementManager(DatabaseContext db)
        {
            _db = db;
            _userService = new UserService(_db);
        }

        public User CreateUser(string email, Guid SSOID)
        {
            try
            {
                var useremail = new System.Net.Mail.MailAddress(email);
            }
            catch (FormatException e)
            {
                throw new InvalidEmailException("Invalid Email", e);
            }
            var _passwordService = new PasswordService();
            DateTime timestamp = DateTime.UtcNow;
            byte[] salt = _passwordService.GenerateSalt();
            string hash = _passwordService.HashPassword(timestamp.ToString(), salt);
            User user = new User
            {
                Username = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UpdatedAt = timestamp,
                Id = SSOID
            };
             _userService.CreateUser(user);
            return user;
        }

        public void DeleteUser(Guid id)
        {
            _userService.DeleteUser(id);
        }

        public User GetUser(Guid id)
        {
            var user = _userService.GetUser(id);
            return user;
        }

        public User GetUser(string email)
        {
            var user = _userService.GetUser(email);
            return user;
        }

        public void DisableUser(User user)
        {
            ToggleUser(user, true);
        }

        public void EnableUser(User user)
        {
            ToggleUser(user, false);
        }

        public void ToggleUser(User user, bool? disable)
        {
            if (disable == null)
            {
                disable = !user.Disabled;
            }
            user.Disabled = (bool)disable;
            _userService.UpdateUser(user);
        }

        public void UpdateUser(User user)
        {
            _userService.UpdateUser(user);
        }
    }
}
