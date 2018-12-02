using System;
using System.Security.Cryptography;

namespace ServiceLayer
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
            byte[] passBytes = System.Text.Encoding.ASCII.GetBytes(password);
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(passBytes, salt,
            1000);
            return hash.GetHashCode().ToString();
        }

        public object CheckPasswordPwned(string password)
        {
            return null;
        }
    }
}