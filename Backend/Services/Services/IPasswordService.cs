using System;

namespace Services
{
    public interface IPasswordService
    {
        byte[] GenerateSalt();
        string HashPassword(string password, byte[] salt);
        object CheckPasswordPwned(string password);
    }
}