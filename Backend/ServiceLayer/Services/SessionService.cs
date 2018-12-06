using System;
using System.Security.Cryptography;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class SessionService : ISessionService
    {
        public SessionService()
        {

        }

        public string GenerateSession()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 /2];
            provider.GetBytes(b);
            string hex = BitConverter.ToString(b).Replace("-","");
            return hex;
        }

        public bool ValidateSession(User user)
        {
            using (var _db = new DatabaseContext())
            {
                Session session = _db.Sessions
                    .Where(s => s.UserId == user.Id && s.ExpiresAt < DateTime.UtcNow)
                    .FirstOrDefault();

                if (session == null)
                    return false;
                return true;
            }
        }
    }
}
