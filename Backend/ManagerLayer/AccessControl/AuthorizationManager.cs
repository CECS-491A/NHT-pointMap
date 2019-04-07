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

        public string ValidateAndUpdateSession(string token, Guid userId)
        {
            using (var _db = CreateDbContext())
            {
                Session response = _sessionService.ValidateSession(_db, token);

                if(response != null)
                {
                    response = _sessionService.UpdateSession(_db, response);
                }
                else
                {
                    return null;
                }

                try
                {
                    _db.SaveChanges();
                    return response.Token;
                }
                catch (DbEntityValidationException ex)
                {
                    //catch error
                    // detach session attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
            }
            return null;
        }

        public int ExpireSession(string token, Guid userId)
        {
            using (var _db = new DatabaseContext())
            {
                Session response = _sessionService.ExpireSession(_db, token);

                return _db.SaveChanges();
            }
        }
        public Session ExpireSession(DatabaseContext _db, string token) {

            return _sessionService.ExpireSession(_db, token);

        }
    }
}
