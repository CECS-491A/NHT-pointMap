using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using DataAccessLayer.Database;
using DataAccessLayer.Models;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        public void Create(User user)
        {
            using(var _db = new DatabaseContext())
            {
                _db.Users.Add(user);
                _db.SaveChanges();
            }
        }

        public void Delete(User user)
        {
            using(var _db = new DatabaseContext())
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        public void DeleteById(Guid Id)
        {
            using(var _db = new DatabaseContext())
            {
                var user = _db.Users.Find(Id);
                if (user != null)
                {
                    _db.Users.Remove(user);
                    _db.SaveChanges();
                }
            }
        }

        public User Get(User user)
        {
            using(var _db = new DatabaseContext())
            {
                return _db.Users.Find(user.Id);
            }
        }

        public User GetById(Guid Id)
        {
            using(var _db = new DatabaseContext())
            {
                return _db.Users.Find(Id);
            }
        }

        public void Update(User user)
        {
            using(var _db = new DatabaseContext())
            {
                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
    }
}
