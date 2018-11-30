using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manager.User
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
