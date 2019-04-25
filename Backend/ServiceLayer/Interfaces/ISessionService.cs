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
        Session CreateSession(Session session, Guid userId);
        Session ValidateSession(string token);
        Session UpdateSession(Session session);
        Session ExpireSession(string  token);
        Session DeleteSession(string token);
    }
}
