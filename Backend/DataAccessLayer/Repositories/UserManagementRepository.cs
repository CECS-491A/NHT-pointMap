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
        DatabaseContext _db;

        public UserManagementRepository(DatabaseContext db)
        {
            _db = db;
        }

        public User CreateNewUser(User user)
        {
            _db.Users.Add(user);
            return user;
        }

        // Notes: use find if searching through pkeys

        public User DeleteUser(Guid Id)
        {
            var user = _db.Users.Find(Id);
            if (user == null)
                return null;
            _db.Users.Remove(user);
            return user;
        }

        public User GetUser(string username)
        {
            var user = _db.Users.Where(u => u.Username == username).FirstOrDefault<User>();
            return user;
        }

        public User GetUser(Guid Id)
        {
            var user = _db.Users.Find(Id);
            return user;
        }

        public User UpdateUser(User updatedUser)
        {
            updatedUser.UpdatedAt = DateTime.UtcNow;
            _db.Entry(updatedUser).State = EntityState.Modified;
            return updatedUser;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _db.Users.ToList<User>();
            return users;
        }
    }
}
