using System;

namespace ServiceLayer.Services
{
    public interface IPasswordService
    {
        byte[] GenerateSalt();
        string HashPassword(string password, byte[] salt);
        object CheckPasswordPwned(string password);
    }
}