using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Data.Entity.Validation;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.AccessControl
{
    public class AuthorizationManager
    {
        private ISessionService _sessionService;
        private IUserService _userService;

        private DatabaseContext CreateDbContext()
        {
            return new DatabaseContext();
        }

        public AuthorizationManager()
        {
             _sessionService = new SessionService();
        }

        public string GenerateSessionToken()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 / 2];
            provider.GetBytes(b);
            string hex = BitConverter.ToString(b).Replace("-", "");
            return hex;
        }

        public Session CreateSession(DatabaseContext _db, User user)
        {
            _userService = new UserService();
            //check if user exist
            var userResponse = _userService.GetUser(_db, user.Username);
            if(userResponse == null)
            {
                return null;
            }
            Session session = new Session();
            session.Token = GenerateSessionToken();
            return _sessionService.CreateSession(_db, session, userResponse.Id);
           
        }

        public string ValidateAndUpdateSession(DatabaseContext _db, string token)
        {
            Session response = _sessionService.ValidateSession(_db, token);

            if(response != null)
            {
                response = _sessionService.UpdateSession(_db, response);
            }

            return null;
        }

        public string ExpireSession(DatabaseContext _db, string token)
        {
            Session response = _sessionService.ExpireSession(_db, token);

            if(response != null)
            {
                return response.Token;
            }
            return null;
        }

        public string DeleteSession(DatabaseContext _db, string token)
        {
            Session response = _sessionService.DeleteSession(_db, token);

            if (response != null)
            {
                return response.Token;
            }
            return null;
        }
    }
}
