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
        public int CreateNewUser(User user)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    _db.Users.Add(user);
                    return _db.SaveChanges();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int DeleteUser(Guid Id)
        {
            using (var _db = new DatabaseContext())
            {
                var user = _db.Users
                    .Where(c => c.Id == Id)
                    .FirstOrDefault<User>();
                if (user == null)
                    return 0;
                _db.Entry(user).State = EntityState.Deleted;
                return _db.SaveChanges();
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

        public int UpdateUser(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            using (var _db = new DatabaseContext())
            {
                try
                {
                    _db.Entry(user).State = EntityState.Modified;
                    return _db.SaveChanges();
                }
                catch (Exception)
                {
                    return 0;
                }
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
