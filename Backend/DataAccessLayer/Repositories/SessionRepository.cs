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
        //returns null if no valid session is found in the sessions table, otherwise
        //  returns the current session
        public Session GetSession(DatabaseContext _db, Guid userId)
        {
            Session session = _db.Sessions
                .Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefault();

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

        public Session CreateSession(DatabaseContext _db, Guid userId)
        {
            User userFind = _db.Users
                .Where(u => u.Id == userId).FirstOrDefault();

            Session session;

            if (userFind == null)
            {
                return null;
            }

            session = GetSession(_db, userId);

            if(session != null)
            {
                return session;
            }
            session = new Session();
            session.Token = GenerateSessionToken();
            session.UserId = userId;

            _db.Entry(session).State = EntityState.Added;
            return session;
        }

        public Session ValidateSession(DatabaseContext _db, string token, Guid userId)
        {
            Session session = GetSession(_db, userId);

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

        public bool DeleteSession(DatabaseContext _db, Guid userId)
        {
            Session session = GetSession(_db, userId);

            if (session != null)
            {
                _db.Entry(session).State = EntityState.Deleted;
            }
            return true;
        }
    }
}
