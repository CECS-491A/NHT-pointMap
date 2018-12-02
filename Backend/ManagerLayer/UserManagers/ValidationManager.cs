using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.UserManagers
{
    public class ValidationManager
    {
        public void CheckPassword(string password)
        {
            IPasswordService _passwordService = new PasswordService();
            object passwordResponse = _passwordService.CheckPasswordPwned(password);
        }
    }
}
