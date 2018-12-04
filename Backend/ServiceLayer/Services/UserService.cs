using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using DataAccessLayer.Database;
using DataAccessLayer.Models;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        public void CreateUser(User user)
        {
            using(var _db = new DatabaseContext())
            {
                _db.Users.Add(user);
                _db.SaveChanges();
            }
        }

        public void DeleteUser(User user)
        {
            using(var _db = new DatabaseContext())
            {
                _db.Entry(user).State = EntityState.Deleted;
                _db.SaveChanges();
            }
        }

        public void DeleteUserById(Guid Id)
        {
            using(var _db = new DatabaseContext())
            {
                var user = _db.Users
                    .Where(c => c.Id == Id)
                    .FirstOrDefault<User>();
                _db.Entry(user).State = EntityState.Deleted;
                _db.SaveChanges();
            }
        }

        public User GetUserByEmail(string email)
        {
            using(var _db = new DatabaseContext())
            {
                var user = _db.Users
                    .Where(c => c.Email == email)
                    .FirstOrDefault<User>();
                return user;
            }
        }

        public User GetUserById(Guid Id)
        {
            using(var _db = new DatabaseContext())
            {
                return _db.Users.Find(Id);
            }
        }

        public void UpdateUser(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            using(var _db = new DatabaseContext())
            {
                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
    }
}
