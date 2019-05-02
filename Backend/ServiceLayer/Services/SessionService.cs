using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;

namespace ServiceLayer.Services
{
    public class SessionService : ISessionService
    {
        private SessionRepository _SessionRepo;
        private DatabaseContext _db;

        public SessionService(DatabaseContext db)
        {
            _SessionRepo = new SessionRepository(db);
            _db = db;
        }

        public Session CreateSession(Session session)
        {
            var sessionResponse = _SessionRepo.CreateSession(session);
            return sessionResponse;
        }

        public Session ValidateSession(string token)
        {
            var session = _SessionRepo.ValidateSession(token);
            return session;
        }

        public Session UpdateSession(Session session)
        {
            var sessionResponse = _SessionRepo.UpdateSession(session);
            return sessionResponse;
        }

        public Session ExpireSession(string token)
        {
            return _SessionRepo.ExpireSession(token);
        }

        public Session DeleteSession(string token)
        {
            var session = _SessionRepo.DeleteSession(token);
            return session;
        }

        public void DeleteSessionsOfUser(Guid userId)
        {
            var sessionsOfUser = _SessionRepo.GetSessions(userId);
            _db.Sessions.RemoveRange(sessionsOfUser);
        }

        public List<Session> GetSessions(Guid userId)
        {
            return _SessionRepo.GetSessions(userId);
        }
    }
}
