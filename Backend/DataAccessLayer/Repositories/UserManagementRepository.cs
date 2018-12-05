using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserManagementRepository
    {

        public void CreateNewUser(User user)
        {
            using (var _db = new DatabaseContext())
            {
                _db.Users.Add(user);
                _db.SaveChanges();
            }
        }

        public void DeleteUser(User user)
        {
            using (var _db = new DatabaseContext())
            {
                _db.Entry(user).State = EntityState.Deleted;
                _db.SaveChanges();
            }
        }

        public void DeleteUser(Guid Id)
        {
            using (var _db = new DatabaseContext())
            {
                var user = _db.Users
                    .Where(c => c.Id == Id)
                    .FirstOrDefault<User>();
                _db.Entry(user).State = EntityState.Deleted;
                _db.SaveChanges();
            }
        }

        public User GetUser(string email)
        {
            using (var _db = new DatabaseContext())
            {
                var user = _db.Users
                    .Where(c => c.Email == email)
                    .FirstOrDefault<User>();
                return user;
            }
        }

        public User GetUser(Guid Id)
        {
            using (var _db = new DatabaseContext())
            {
                return _db.Users.Find(Id);
            }
        }

        public void UpdateUser(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            using (var _db = new DatabaseContext())
            {
                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public bool ExistingUser(User user)
        {
            var result = GetUser(user.Email);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public bool ExistingUser(string email)
        {
            var result = GetUser(email);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
