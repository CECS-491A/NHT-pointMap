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

        public Session CreateSession(Session session, Guid userId)
        {
            return _SessionRepo.CreateSession(session, userId);
        }

        public Session ValidateSession(string token)
        {
            return _SessionRepo.ValidateSession(token);
        }

        public Session UpdateSession(Session session)
        {
            return _SessionRepo.UpdateSession(session);
        }

        public Session ExpireSession(string token)
        {
            return _SessionRepo.ExpireSession(token);
        }

        public Session DeleteSession(string token)
        {
            return _SessionRepo.DeleteSession(token);
        }

        public void DeleteSessionsOfUser(Guid userId)
        {
            var sessionsOfUser = _SessionRepo.GetSessions(userId);
            _db.Sessions.RemoveRange(sessionsOfUser);
        }

        public Session GetSession(string token)
        {
            return _SessionRepo.GetSession(token);
        }

        public List<Session> GetSessions(Guid userId)
        {
            return _SessionRepo.GetSessions(userId);
        }
    }
}
