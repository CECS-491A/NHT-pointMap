using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

using System.Security.Cryptography;
using System.Text;
using ServiceLayer.KFC_API_Services;

namespace UnitTesting
{
    public class TestingUtils
    {
        public string Mock_APISecret = SSO_APIService.APISecret;

        public byte[] GetRandomness()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public Point CreatePointObject(float longitude, float latitude)
        {
            Random rand = new Random();
            Point p = new Point
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid() + " ",
                Longitude = longitude,
                Latitude = latitude,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return p;
        }

        public Point CreatePointObject()
        {
            Random rand = new Random();
            Point p = new Point
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid() + " ",
                Longitude = (float)rand.NextDouble() * 360 - 180,
                Latitude = (float)rand.NextDouble() * 180 - 90,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return p;
        }

        public Point CreatePointInDb()
        {
            Random rand = new Random();
            Point p = new Point
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid() + " ",
                Longitude = (float)rand.NextDouble() * 360 - 180,
                Latitude = (float)rand.NextDouble() * 180 - 90,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return CreatePointInDb(p);
        }

        public Point CreatePointInDb(Point point)
        {
            using (var _db = new DatabaseContext())
            {
                _db.Entry(point).State = System.Data.Entity.EntityState.Added;
                _db.SaveChanges();

                return point;
            }
        }

        public User CreateUserInDb()
        {

            User u = new User
            {
                Id = Guid.NewGuid(),
                Username = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = GetRandomness()
            };

            return CreateUserInDb(u);
        }

        public User CreateUserInDb(User user)
        {
            using (var _db = new DatabaseContext())
            {
                _db.Entry(user).State = System.Data.Entity.EntityState.Added;
                _db.SaveChanges();
                return user;
            }
        }

        public string GenerateTokenSignature(Guid ssoUserId, string email, long timestamp)
        {
            string preSignatureString = "";
            preSignatureString += "ssoUserId=" + ssoUserId.ToString() + ";";
            preSignatureString += "email=" + email + ";";
            preSignatureString += "timestamp=" + timestamp + ";";
            HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(Mock_APISecret));
            byte[] launchPayloadBuffer = Encoding.ASCII.GetBytes(preSignatureString);
            byte[] signatureBytes = hmacsha1.ComputeHash(launchPayloadBuffer);
            string signature = Convert.ToBase64String(signatureBytes);
            return signature;
        }

        public class MockLoginPayload
        {
            public string Mock_APISecret = SSO_APIService.APISecret;

            public Guid ssoUserId { get; set; }
            public string email { get; set; }
            public long timestamp { get; set; }

            public string Signature()
            {
                var _ssoAuth = new SignatureService();
                var payload = _ssoAuth.PreparePayload(ssoUserId.ToString(), email, timestamp);
                var signature = _ssoAuth.Sign(payload);
                return signature;
            }

            public string PreSignatureString()
            {
                string preSignatureString = "";
                preSignatureString += "ssoUserId=" + ssoUserId.ToString() + ";";
                preSignatureString += "email=" + email + ";";
                preSignatureString += "timestamp=" + timestamp + ";";
                return preSignatureString;
            }

        }

        public MockLoginPayload GenerateLoginPayloadWithSignature(Guid ssoUserId, string email, long timestamp)
        {
            MockLoginPayload mock_payload = new MockLoginPayload();
            mock_payload.ssoUserId = ssoUserId;
            mock_payload.email = email;
            mock_payload.timestamp = timestamp;
            return mock_payload;
        }

        public string GeneratePreSignatureString(Guid ssoUserId, string email, long timestamp)
        {
            string preSignatureString = "";
            preSignatureString += "ssoUserId=" + ssoUserId.ToString() + ";";
            preSignatureString += "email=" + email + ";";
            preSignatureString += "timestamp=" + timestamp + ";";
            return preSignatureString;
        }

        public User CreateSSOUserInDb()
        {
            User user = new User
            {
                Username = Guid.NewGuid() + "@mail.com",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = GetRandomness(),
                UpdatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid()
            };
            return CreateUserInDb(user);
        }

        public User CreateUserObject()
        {
            User user = new User
            {
                Id = Guid.NewGuid(),
                Username = Guid.NewGuid() + "@" + Guid.NewGuid() + ".com",
                City = "Los Angeles",
                State = "California",
                Country = "United States",
                PasswordHash = (Guid.NewGuid()).ToString(),
                PasswordSalt = GetRandomness()
            };
            return user;
        }
        
        public Session CreateSessionObject(User user)
        {
            Session session = new Session
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Session.MINUTES_UNTIL_EXPIRATION),
                User = user,
                Token = (Guid.NewGuid()).ToString()
            };
            return session;
        }

        public Session CreateSessionInDb(Session session)
        {
            using (var _db = new DatabaseContext())
            {
                _db.Entry(session).State = System.Data.Entity.EntityState.Added;
                _db.SaveChanges();

                return session;
            }
        }
        
        public Service CreateServiceInDb(bool enabled)
        {
            using (var _db = new DatabaseContext())
            {
                Service s = new Service
                {
                    ServiceName = (Guid.NewGuid()).ToString(),
                    Disabled = !enabled,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.Services.Add(s);
                _db.SaveChanges();

                return s;
            }
        }

        public Service CreateServiceObject(bool enabled)
        {
            Service s = new Service
            {
                ServiceName = (Guid.NewGuid()).ToString(),
                Disabled = !enabled
            };

            return s;
        }

        public Claim CreateClaim(User user, Service service, User subjectUser)
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

        public Client CreateClientObject() {
            Client client = new Client
            {
                Id = Guid.NewGuid(),
                Disabled = false,
                Name = Guid.NewGuid().ToString(),
                Address = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow

            };
            return client;
        }

        public Client CreateClientInDb()
        {

            Client client = new Client
            {
                Id = Guid.NewGuid(),
                Disabled = false,
                Name = Guid.NewGuid().ToString(),
                Address = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow

            };

            return CreateClientInDb(client);
        }

        public Client CreateClientInDb(Client client)
        {
            using (var _db = new DatabaseContext())
            {
                _db.Clients.Add(client);
                _db.SaveChanges();

                return client;
            }
        }

        public DatabaseContext CreateDataBaseContext()
        {
            return new DatabaseContext();
        }

        public bool isEqual(string[] arr1, string[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }
            return true;
        }
    }

}
