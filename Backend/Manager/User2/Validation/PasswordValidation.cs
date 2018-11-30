using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manager.User.Validation
{
    public class PasswordValidation
    {
        PasswordService _passwordService;

        public void CheckPassword(string password)
        {
            _passwordService = new PasswordService();
            object passwordResponse = _passwordService.CheckPasswordPwned(password);
        }
    }
}
