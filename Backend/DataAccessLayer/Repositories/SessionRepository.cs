using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public class SessionRepository
    {
        private DatabaseContext _db;

        public SessionRepository(DatabaseContext db)
        {
            _db = db;
        }
        //returns null if no valid session is found in the sessions table, otherwise
        //  returns the current session
        public Session GetSession(string token)
        {
            var session = _db.Sessions
                .Where(s => s.Token == token)
                .FirstOrDefault<Session>();

            return session;
        }
        
        public Session CreateSession(Session session, Guid userId)
        {
            session.UserId = userId;
            _db.Sessions.Add(session);
            return session;
        }

        public Session ValidateSession(string token)
        {
            var session = GetSession(token);

            //checks if session exists and if it is already expired
            if (session == null || session.Token != token || session.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }
            else
            {
                return session;
            }
        }

        public Session DeleteSession(string token)
        {
            var session = _db.Sessions
                .Where(s => s.Token == token)
                .FirstOrDefault<Session>();
            if (session == null)
                return null;
            _db.Sessions.Remove(session);
            return session;
        }

        public Session UpdateSession(Session session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            session.ExpiresAt = DateTime.UtcNow.AddMinutes(Session.MINUTES_UNTIL_EXPIRATION);
            _db.Entry(session).State = EntityState.Modified;
            return session;
        }

        public Session ExpireSession(string token)
        {
            var session = GetSession(token);
            if (session == null)
                return null;

            session.UpdatedAt = DateTime.UtcNow;
            session.ExpiresAt = DateTime.UtcNow;
            _db.Entry(session).State = EntityState.Modified;
            return session;
        }

        public List<Session> GetSessions(Guid userId)
        {
            var sessions = _db.Sessions
                .Where(s => s.UserId == userId)
                .ToList();
            return sessions;
        }
    }
}
