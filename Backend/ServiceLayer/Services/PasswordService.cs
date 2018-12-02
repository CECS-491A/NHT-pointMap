using System;
using System.Security.Cryptography;

namespace ServiceLayer.Services
{
    public class PasswordService : IPasswordService
    {
        public byte[] GenerateSalt() {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public string HashPassword(string password, byte[] salt)
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt);
            rfc.IterationCount = 10000;
            byte[] hash = rfc.GetBytes(16);
            return Convert.ToBase64String(hash);
        }

        public object CheckPasswordPwned(string password)
        {
            return null;
        }
    }
}