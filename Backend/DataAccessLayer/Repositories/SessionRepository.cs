using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System;
using System.Data.Entity;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class SessionRepository
    {
        //returns null if no valid session is found in the sessions table, otherwise
        //  returns the current session
        public Session GetSession(DatabaseContext _db, string token)
        {
            var session = _db.Sessions
                .Where(s => s.Token == token)
                .FirstOrDefault<Session>();

            return session;
        }

        public Session GetSession(DatabaseContext _db, User user)
        {
            var session = _db.Sessions
                          .Where(s => s.UserId == user.Id)
                          .FirstOrDefault<Session>();
            return session;
        }

        public Session CreateSession(DatabaseContext _db, Session session, Guid userId)
        {
            session.UserId = userId;
            _db.Entry(session).State = EntityState.Added;
            return session;
        }

        public Session ValidateSession(DatabaseContext _db, string token)
        {
            var session = GetSession(_db, token);

            if (session == null || session.Token != token)
            {
                return null;
            }
            else if (session.ExpiresAt < DateTime.UtcNow)
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
            session.UpdatedAt = DateTime.UtcNow;
            session.ExpiresAt = DateTime.UtcNow.AddMinutes(Session.MINUTES_UNTIL_EXPIRATION);
            _db.Entry(session).State = EntityState.Modified;
            return session;
        }

        public Session ExpireSession(DatabaseContext _db, string token)
        {
            var session = GetSession(_db, token);
            if (session == null)
                return null;

            session.UpdatedAt = DateTime.UtcNow;
            session.ExpiresAt = DateTime.UtcNow;
            _db.Entry(session).State = EntityState.Modified;
            return session;
        }

        public Guid GetSessionGuid(DatabaseContext _db, string token)
        {
            return GetSession(_db, token).Id;
        }

       
    }
}
