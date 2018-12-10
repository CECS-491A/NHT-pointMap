using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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

        public int CreateUser(User user)
        {
            if (_UserManagementRepo.ExistingUser(user))
            {
                Console.WriteLine("User exists");
                return 0;
            }
            return _UserManagementRepo.CreateNewUser(user);
        }

        public int DeleteUser(Guid Id)
        {
            return _UserManagementRepo.DeleteUser(Id);
        }

        public User GetUser(string email)
        {
            return _UserManagementRepo.GetUser(email);
        }

        public User GetUser(Guid Id)
        {
            return _UserManagementRepo.GetUser(Id);
        }

        public int UpdateUser(User user)
        {
            return _UserManagementRepo.UpdateUser(user);
        }

        public bool IsManagerOf(User user, User subject)
        {
            return _UserManagementRepo.IsManagerOf(user, subject);
        }

        public User Login(string email, string password)
        {
            UserRepository userRepo = new UserRepository();
            PasswordService _passwordService = new PasswordService();
            var user = _UserManagementRepo.GetUser(email);
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
