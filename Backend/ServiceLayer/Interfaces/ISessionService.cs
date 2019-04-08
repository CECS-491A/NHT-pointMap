using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public interface ISessionService
    {
        Session CreateSession(DatabaseContext _db, Session session, Guid userId);
        Session ValidateSession(DatabaseContext _db, string token);
        Session UpdateSession(DatabaseContext _db, Session session);
        Session ExpireSession(DatabaseContext _db, string  token);
        Session DeleteSession(DatabaseContext _db, string token);
    }
}
