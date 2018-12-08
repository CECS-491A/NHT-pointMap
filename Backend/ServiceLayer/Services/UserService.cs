using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private UserManagementRepository _UserManagementRepo;

        public UserService()
        {
            _UserManagementRepo = new UserManagementRepository();
        }

        public User CreateUser(User user, DatabaseContext _db)
        {
            if (_UserManagementRepo.ExistingUser(user, _db))
            {
                Console.WriteLine("User exists");
                return null;
            }
            return _UserManagementRepo.CreateNewUser(user, _db);
        }

        public User DeleteUser(Guid Id, DatabaseContext _db)
        {
            return _UserManagementRepo.DeleteUser(Id, _db);
        }

        public User GetUser(string email, DatabaseContext _db)
        {
            return _UserManagementRepo.GetUser(email, _db);
        }

        public User GetUser(Guid Id, DatabaseContext _db)
        {
            return _UserManagementRepo.GetUser(Id, _db);
        }

        public User UpdateUser(User user, DatabaseContext _db)
        {
            return _UserManagementRepo.UpdateUser(user, _db);
        }

        public User Login(string email, string password, DatabaseContext _db)
        {
            UserRepository userRepo = new UserRepository();
            PasswordService _passwordService = new PasswordService();
            var user = _UserManagementRepo.GetUser(email, _db);
            if (user != null)
            {
                string hashedPassword = _passwordService.HashPassword(password, user.PasswordSalt);
                if (userRepo.ValidatePassword(user, hashedPassword))
                {
                    Console.WriteLine("Password Correct");
                    return user;
                }
                Console.WriteLine("Password Incorrect");
                return null;
            }
            Console.WriteLine("User does not exist");
            return null;
        }
    }
}
