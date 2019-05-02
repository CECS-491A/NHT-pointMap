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
        DatabaseContext _db;

        public AuthorizationManager(DatabaseContext db)
        {
             _sessionService = new SessionService();
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
            Session session = new Session();
            session.Token = GenerateSessionToken();
            session = _sessionService.CreateSession(_db, session, userResponse.Id);
            return session;
        }

        public Session ValidateAndUpdateSession(DatabaseContext _db, string token)
        {
            Session response = _sessionService.ValidateSession(_db, token);

            if(response != null)
            {
                response = _sessionService.UpdateSession(_db, response);
            }
            return response;
        }

        public Session ExpireSession(DatabaseContext _db, string token)
        {
            Session response = _sessionService.ExpireSession(_db, token);
            return response;
        }

        public Session DeleteSession(DatabaseContext _db, string token)
        {
            Session response = _sessionService.DeleteSession(_db, token);
            return response;
        }
    }
}
