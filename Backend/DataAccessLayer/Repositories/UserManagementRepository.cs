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
        public User CreateNewUser(User user, DatabaseContext _db)
        {
            _db.Entry(user).State = EntityState.Added;
            return user;
        }

        public User DeleteUser(Guid Id, DatabaseContext _db)
        {
            var user = _db.Users
                .Where(c => c.Id == Id)
                .FirstOrDefault<User>();
            if (user == null)
                return null;
            _db.Entry(user).State = EntityState.Deleted;
            return user;
        }

        public User GetUser(string email, DatabaseContext _db)
        {
            var user = _db.Users
                .Where(c => c.Email == email)
                .FirstOrDefault<User>();
            return user;
        }

        public User GetUser(Guid Id, DatabaseContext _db)
        {
            return _db.Users.Find(Id);
        }

        public User UpdateUser(User user, DatabaseContext _db)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _db.Entry(user).State = EntityState.Modified;
            return user;
        }

        public bool ExistingUser(User user, DatabaseContext _db)
        {
            var result = GetUser(user.Email, _db);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public bool ExistingUser(string email, DatabaseContext _db)
        {
            var result = GetUser(email, _db);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
