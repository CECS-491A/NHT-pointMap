using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public Session GenerateSession(DatabaseContext _db, Guid userId)
        {
            return _SessionRepo.CreateSession(_db, userId);
        }

        //Also updates the expiration of the session
        public Session ValidateSession(DatabaseContext _db, string token, User user)
        {
            return _SessionRepo.ValidateSession(_db, token, user.Id);
        }

        public Session UpdateSession(DatabaseContext _db, Session session)
        {
            return _SessionRepo.UpdateSession(_db, session);
        }
    }
}
