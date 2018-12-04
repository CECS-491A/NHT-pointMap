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
        void Create(User user);
        User GetById(Guid Id);
        User Get(User user); 
        void Delete(User user);
        void DeleteById(Guid Id);
        void Update(User user);
    }
}
