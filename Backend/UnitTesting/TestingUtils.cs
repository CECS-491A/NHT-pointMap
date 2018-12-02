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
        public User createUser()
        {
            using (var _db = new DatabaseContext())
            {
                User u = new User
                {
                    Id = new Guid(),
                    Email = new Guid() + "@" + new Guid() + ".com",
                    DateOfBirth = DateTime.UtcNow,
                    City = "Los Angeles",
                    State = "California",
                    Country = "United States",
                    PasswordHash = (new Guid()).ToString(),
                    PasswordSalt = new byte[128 / 8]
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
                    Id = new Guid(),
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow
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
                    Id = new Guid(),
                    ServiceName = (new Guid()).ToString(),
                    Disable = enabled
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
                    Id = new Guid(),
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
