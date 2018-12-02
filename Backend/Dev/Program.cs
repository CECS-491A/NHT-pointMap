using DataAccessLayer.Data;
using DataAccessLayer.Models;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            IPasswordService _passwordService = new PasswordService();
            Guid guid = new Guid(_passwordService.GenerateSalt());
            Console.WriteLine(guid);

            using (var _db = new DatabaseContext())
            {
                DateTime timestamp = DateTime.UtcNow;
                DateTime dob = DateTime.Now.Date;
                string userPassword = "mypasswordisnowhashedandhidden";
                byte[] salt = _passwordService.GenerateSalt();
                string hash = _passwordService.HashPassword(userPassword, salt);
                var u = new User
                {
                    Id = guid,
                    Email = "sauce@mail.com",
                    UpdatedAt = timestamp,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    DateOfBirth = dob
                };
                _db.Users.Add(u);
                _db.SaveChanges();
                var users = _db.Users;

                foreach (var user in users.ToList())
                {
                    Console.WriteLine(user.Email, " ", user.PasswordHash.Length);
                }
                Console.ReadKey();
            }
        }
    }
}
