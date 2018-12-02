using System;
using System.Security.Cryptography;
 // will need to remove this

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
            //string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            //    password: password,
            //    salt: salt,
            //    prf: KeyDerivationPrf.HMACSHA1,
            //    iterationCount: 10000,
            //    numBytesRequested: 256 / 8));
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