using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.User.Management
{
    public class UserManagementLayer
    {
        private IPasswordService _passwordService;
        
        public void CreateUser()
        {
            _passwordService = new PasswordService();
        }

        public void DeleteUser()
        {

        }

        public void GetUser()
        {

        }

        public void UpdaetUser()
        {

        }
    }
}
