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
                    Email = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                    DateOfBirth = DateTime.UtcNow,
                    City = "Los Angeles",
                    State = "California",
                    Country = "United States",
                    PasswordHash = (Guid.NewGuid()).ToString(),
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
                    ServiceName = (new Guid()).ToString(),
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
