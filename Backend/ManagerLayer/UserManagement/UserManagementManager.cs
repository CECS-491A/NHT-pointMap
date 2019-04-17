using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.UserManagement
{
    public class UserManagementManager
    {
        private IPasswordService _passwordService;
        private IUserService _userService;

        public UserManagementManager()
        {
            _userService = new UserService();
        }

        public User CreateUser(DatabaseContext _db, string email, Guid SSOID)
        {
            try
            {
                var useremail = new System.Net.Mail.MailAddress(email);
            }
            catch (FormatException e)
            {
                throw new InvalidEmailException("Invalid Email", e);
            }
            _passwordService = new PasswordService();
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
             _userService.CreateUser(_db, user);
            return user;
        }

        public void DeleteUser(DatabaseContext _db, Guid id)
        {
            _userService.DeleteUser(_db, id);
        }

        public User GetUser(DatabaseContext _db, Guid id)
        {
            return _userService.GetUser(_db, id); 
        }

        public User GetUser(DatabaseContext _db, string email)
        {
            return _userService.GetUser(_db, email);
        }

        public void DisableUser(DatabaseContext _db, User user)
        {
            ToggleUser(_db, user, true);
        }

        public void EnableUser(DatabaseContext _db, User user)
        {
            ToggleUser(_db, user, false);
        }

        public void ToggleUser(DatabaseContext _db, User user, bool? disable)
        {
            if (disable == null)
            {
                disable = !user.Disabled;
            }
            user.Disabled = (bool)disable;
            _userService.UpdateUser(_db, user);
        }

        public void UpdateUser(DatabaseContext _db, User user)
        {
            _userService.UpdateUser(_db, user);
        }
    }
}
