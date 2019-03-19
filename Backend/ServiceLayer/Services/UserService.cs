using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;


namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private UserManagementRepository _UserManagementRepo;

        public UserService()
        {
            _UserManagementRepo = new UserManagementRepository();
        }

        public User CreateUser(DatabaseContext _db, User user)
        {
            if (_UserManagementRepo.ExistingUser(_db, user))
            {
                Console.WriteLine("User exists");
                return null;
            }
            return _UserManagementRepo.CreateNewUser(_db, user);
        }

        public User DeleteUser(DatabaseContext _db, Guid Id)
        {
            return _UserManagementRepo.DeleteUser(_db, Id);
        }

        public User GetUser(DatabaseContext _db, string email)
        {
            return _UserManagementRepo.GetUser(_db, email);
        }

        public User GetUser(DatabaseContext _db, Guid Id)
        {
            return _UserManagementRepo.GetUser(_db, Id);
        }

        public User GetUserBySSOID(DatabaseContext _db, Guid SSOID)
        {
            return _UserManagementRepo.GetUserBySSOID(_db, SSOID);
        }

        public User UpdateUser(DatabaseContext _db, User user)
        {
            return _UserManagementRepo.UpdateUser(_db, user);
        }

        public bool IsManagerOver(DatabaseContext _db, User user, User subject)
        {
            return _UserManagementRepo.IsManagerOver(_db, user, subject);
        }

    }
}
