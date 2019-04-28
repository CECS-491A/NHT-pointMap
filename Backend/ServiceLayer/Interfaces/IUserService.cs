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
        User CreateUser(User user);
        User GetUser(string email);
        User GetUser(Guid Id);
        User DeleteUser(Guid Id);
        User UpdateUser(User user);
        bool IsManagerOver(User user, User subject);
    }
}
