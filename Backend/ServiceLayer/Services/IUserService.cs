using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public interface IUserService
    {
        // CRUD
        User CreateUser(User user, DatabaseContext _db);
        User GetUser(string email, DatabaseContext _db);
        User GetUser(Guid Id, DatabaseContext _db);
        User DeleteUser(Guid Id, DatabaseContext _db);
        User UpdateUser(User user, DatabaseContext _db);
    }
}
