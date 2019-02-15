using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class SessionRepository
    {
        public Session CreateSession(DatabaseContext _db, Guid userId)
        {
            Session session = new Session();
            session.Token = GenerateSessionToken();
            session.UserId = userId;

            _db.Entry(session).State = EntityState.Added;
            return session;
        }

        public string GenerateSessionToken()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 / 2];
            provider.GetBytes(b);
            string hex = BitConverter.ToString(b).Replace("-", "");
            return hex;
        }

        public Session ValidateSession(DatabaseContext _db, string token, Guid userId)
        {
            Session session = _db.Sessions
                .Where(s => s.UserId == userId && s.ExpiresAt < DateTime.UtcNow)
                .FirstOrDefault();

            if (session == null || session.Token != token)
            {
                return null;
            }
            else
            {
                return session;
            }
        }

        public Session UpdateSession(DatabaseContext _db, Session session)
        {
            if (session == null)
            {
                return null;
            }
            else
            {
                session.UpdatedAt = DateTime.UtcNow;
                session.ExpiresAt = DateTime.UtcNow.AddMinutes(Session.MINUTES_UNTIL_EXPIRATION);

                _db.Entry(session).State = EntityState.Modified;

                return session;
            }
        }
    }
}
