using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

using System.Security.Cryptography;

namespace UnitTesting
{
    public class TestingUtils
    {
        public byte[] GetRandomness()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public User CreateUserInDb()
        {
            using (var _db = new DatabaseContext())
            { 
                User u = new User
                {
                    Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                    DateOfBirth = DateTime.UtcNow,
                    City = "Los Angeles",
                    State = "California",
                    Country = "United States",
                    PasswordHash = (Guid.NewGuid()).ToString(),
                    PasswordSalt = GetRandomness()
                };
                _db.Users.Add(u);
                _db.SaveChanges();

                return u;
            }
        }

        public User CreateUserObject()
        {
            User user = new User
            {
                Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                DateOfBirth = DateTime.UtcNow,
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = GetRandomness()
            };
            return user;
        }

        public Session CreateSession(User user)
        {
           using (var _db = new DatabaseContext())
            {
                Session s = new Session
                {
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow,
                    Token = "token"
                };
                _db.Sessions.Add(s);
                _db.SaveChanges();

                return s;
            }
        }

        public Service CreateService(bool enabled)
        {
            using (var _db = new DatabaseContext())
            {
                Service s = new Service
                {
                    ServiceName = (Guid.NewGuid()).ToString(),
                    Disabled = !enabled
                };
                _db.Services.Add(s);
                _db.SaveChanges();

                return s;
            }
        }

        public Claim CreateClaim(User user, Service service, User subjectUser)
        {
            using (var _db = new DatabaseContext())
            {
                Claim c = new Claim
                {
                    ServiceId = service.Id,
                    UserId = user.Id,
                    SubjectUserId = subjectUser.Id
                };
                _db.Claims.Add(c);
                _db.SaveChanges();

                return c;
            }
        }

        public DatabaseContext CreateDataBaseContext()
        {
            return new DatabaseContext();
        }
    }
}
