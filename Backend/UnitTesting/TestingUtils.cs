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
        public byte[] getRandomness()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        public User createUser()
        {
            using (var _db = new DatabaseContext())
            { 
                User u = new User
                {
                    Id = new Guid(getRandomness()),
                    Email = new Guid() + "@" + new Guid() + ".com",
                    DateOfBirth = DateTime.UtcNow,
                    City = "Los Angeles",
                    State = "California",
                    Country = "United States",
                    PasswordHash = (new Guid()).ToString(),
                    PasswordSalt = getRandomness()
                };
                _db.Users.Add(u);
                _db.SaveChanges();

                return u;
            }
        }

        public Session createSession(User user)
        {
           using (var _db = new DatabaseContext())
            {
                Session s = new Session
                {
                    Id = new Guid(getRandomness()),
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow,
                    Token = "token"
                };
                _db.Sessions.Add(s);
                _db.SaveChanges();

                return s;
            }
        }

        public Service createService(bool enabled)
        {
            using (var _db = new DatabaseContext())
            {
                Service s = new Service
                {
                    Id = new Guid(getRandomness()),
                    ServiceName = (new Guid(getRandomness())).ToString(),
                    Disabled = enabled
                };
                _db.Services.Add(s);
                _db.SaveChanges();

                return s;
            }
        }

        public Claim createClaim(User user, Service service)
        {
            using (var _db = new DatabaseContext())
            {
                Claim c = new Claim
                {
                    Id = new Guid(getRandomness()),
                    ServiceId = service.Id,
                    UserId = user.Id
                };
                _db.Claims.Add(c);
                _db.SaveChanges();

                return c;
            }
        }
    }
}
