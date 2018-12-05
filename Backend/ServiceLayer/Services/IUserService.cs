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
        void CreateUser(User user);
        User GetUser(string email);
        User GetUser(Guid Id);
        void DeleteUser(Guid Id);
        void UpdateUser(User user);
    }
}
