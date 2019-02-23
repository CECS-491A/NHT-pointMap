using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace ServiceLayer.Services
{
    public class SessionService : ISessionService
    {
        private SessionRepository _SessionRepo;

        public SessionService()
        {
            _SessionRepo = new SessionRepository();
        }

        public Session CreateSession(DatabaseContext _db, Session session, Guid userId)
        {
            return _SessionRepo.CreateSession(_db, session, userId);
        }

        public Session ValidateSession(DatabaseContext _db, string token)
        {
            return _SessionRepo.ValidateSession(_db, token);
        }

        public Session UpdateSession(DatabaseContext _db, Session session)
        {
            return _SessionRepo.UpdateSession(_db, session);
        }

        public Session DeleteSession(DatabaseContext _db, string token)
        {
            return _SessionRepo.DeleteSession(_db, token);
        }

        public Session GetSession(DatabaseContext _db, string token)
        {
            return _SessionRepo.GetSession(_db, token);
        }
    }
}
