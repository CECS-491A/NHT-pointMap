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
        DatabaseContext _db;

        public UserService(DatabaseContext db)
        {
            _UserManagementRepo = new UserManagementRepository(db);
            _db = db;
        }

        public User CreateUser(User user)
        {
            return _UserManagementRepo.CreateNewUser(user);
        }

        public User DeleteUser(Guid Id)
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

        public User UpdateUser(User user)
        {
            return _UserManagementRepo.UpdateUser(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _UserManagementRepo.GetAllUsers();
        }

        public bool IsManagerOver(User user, User subject)
        {
            if (subject.ManagerId == null)
            {
                return false;
            }
            if (user.Id == subject.ManagerId)
            {
                return true;
            }
            var subjectManager = GetUser(subject.ManagerId.Value);
            if (subjectManager != null)
            {
                return IsManagerOver(user, subjectManager);
            }
            return false;
        }

    }
}
