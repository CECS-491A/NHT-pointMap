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
        User GetUserByEmail(string email);
        User GetUserById(Guid Id);
        void DeleteUser(User user);
        void DeleteUserById(Guid Id);
        void UpdateUser(User user);
    }
}
