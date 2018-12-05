using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.UserManagers
{
    public class UserManager
    {
        public void Register()
        {

        }

        public void Login(string email, string password)
        {
            UserService _userService = new UserService();
            PasswordService _passwordService = new PasswordService();
            var user = _userService.Login(email, password);
        }
    }
}
