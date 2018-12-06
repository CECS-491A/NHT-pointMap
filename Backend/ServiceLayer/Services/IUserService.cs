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
        int CreateUser(User user);
        User GetUser(string email);
        User GetUser(Guid Id);
        int DeleteUser(Guid Id);
        int UpdateUser(User user);
    }
}
