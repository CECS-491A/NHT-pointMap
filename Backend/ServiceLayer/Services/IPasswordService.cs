using System;

namespace ServiceLayer
{
    public interface IPasswordService
    {
        byte[] GenerateSalt();
        string HashPassword(string password, byte[] salt);
    }
}