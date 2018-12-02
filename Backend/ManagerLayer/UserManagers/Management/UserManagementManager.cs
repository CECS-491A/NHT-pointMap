using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.UserManagers.Management
{
    public class UserManagementManager
    {
        private IPasswordService _passwordService;

        public void CreateUser()
        {
            _passwordService = new PasswordService();
            // this db stuff should go in user services
            //using (var _db = new DatabaseContext())
            //{
            //    DateTime dob = DateTime.Now.Date;
            //    string userPassword = "testuser4password";
            //    byte[] salt = _passwordService.GenerateSalt();
            //    string hash = _passwordService.HashPassword(userPassword, salt);
            //    Console.WriteLine(hash);
            //}
        }

        public void DeleteUser()
        {

        }

        public void GetUser()
        {

        }

        public void UpdateUser()
        {

        }
    }
}
