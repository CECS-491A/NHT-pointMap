using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Security.Cryptography;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.AccessControl
{
    public class AuthorizationManager
    {
        private ISessionService _sessionService;
        private IUserService _userService;
        private DatabaseContext _db;

        public AuthorizationManager(DatabaseContext db)
        {
             _sessionService = new SessionService(db);
            _db = db;
        }

        public string GenerateSessionToken()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 / 2];
            provider.GetBytes(b);
            string hex = BitConverter.ToString(b).Replace("-", "");
            return hex;
        }

        public Session CreateSession(User user)
        {
            _userService = new UserService(_db);
            //check if user exist
            var userResponse = _userService.GetUser(user.Id);
            if(userResponse == null)
            {
                throw new UserNotFoundException("User does not exist. Session can not be created.");
            }
            var session = new Session();
            session.Token = GenerateSessionToken();
            session = _sessionService.CreateSession(session, userResponse.Id);
            return session;
        }

        public Session ValidateAndUpdateSession(string token)
        {
            var session = _sessionService.ValidateSession(token);

            if(session != null)
            {
                session = _sessionService.UpdateSession(session);
            }

            return session;
        }

        public Session ExpireSession(string token)
        {
            var session = _sessionService.ExpireSession(token);

            return session;
        }

        public Session DeleteSession(string token)
        {
            var session = _sessionService.DeleteSession(token);

            return session;
        }
    }
}
