using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.UserManagement
{
    public class UserManagementManager
    {
        private IPasswordService _passwordService;
        private IUserService _userService;
        private readonly DatabaseContext _db;

        public UserManagementManager()
        {
            _userService = new UserService();
        }

        public UserManagementManager(DatabaseContext db)
        {
            _db = db;
            _userService = new UserService();
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

        public void DeleteUser(Guid id)
        {
            _userService.DeleteUser(_db, id);
        }

        public User GetUser(Guid id)
        {
            return _userService.GetUser(_db, id); 
        }

        public User GetUser(string email)
        {
            return _userService.GetUser(_db, email);
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
            _userService.UpdateUser(_db, user);
        }

        public void UpdateUser(User user)
        {
            _userService.UpdateUser(_db, user);
        }
    }
}
