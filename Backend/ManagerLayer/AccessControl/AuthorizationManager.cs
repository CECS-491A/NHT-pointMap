using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Security.Cryptography;

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
            var userResponse = _userService.GetUser(user.Username);
            if(userResponse == null)
            {
                return null;
            }
            var session = new Session();
            session.Token = GenerateSessionToken();
            session = _sessionService.CreateSession(session, userResponse.Id);
            return session;
        }

        public Session ValidateAndUpdateSession(string token)
        {
            var response = _sessionService.ValidateSession(token);

            if(response != null)
            {
                response = _sessionService.UpdateSession(response);
            }

            return response;
        }

        public Session ExpireSession(string token)
        {
            var response = _sessionService.ExpireSession(token);

            return response;
        }

        public Session DeleteSession(string token)
        {
            var response = _sessionService.DeleteSession(token);

            return response;
        }
    }
}
