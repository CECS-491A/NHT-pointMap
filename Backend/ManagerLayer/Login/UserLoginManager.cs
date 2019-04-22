using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.AccessControl;
using DTO;
using ManagerLayer.UserManagement;
using ManagerLayer.Logging;
using ServiceLayer.Services;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.Login
{
    public class UserLoginManager
    {
        UserManagementManager _userManagementManager;
        AuthorizationManager _authorizationManager;
        TokenService _tokenService;
        LogRequestDTO newLog;
        LoggingManager loggingManager;
        DatabaseContext _db;

        public UserLoginManager(DatabaseContext db)
        {
            _db = db;
        }

        public LoginManagerResponseDTO LoginFromSSO(string Username, Guid ssoID, string Signature, string PreSignatureString)
        {
            ////////////////////////////////////////
            /// User oAuth at the indivudal application level
            // verify if the login payload is valid via its signature
            _tokenService = new TokenService();
            loggingManager = new LoggingManager();
            if (!_tokenService.isValidSignature(PreSignatureString, Signature))
            {
                throw new InvalidTokenSignatureException("Session is not valid.");
            }
            ////////////////////////////////////////
            
            _userManagementManager = new UserManagementManager(_db);
            var user = _userManagementManager.GetUser(ssoID);
            // check if user does not exist
            if (user == null)
            {
                // create new user
                user = _userManagementManager.CreateUser(_db, Username, ssoID);
            }
            _authorizationManager = new AuthorizationManager(_db);
            Session session = _authorizationManager.CreateSession(user);

            LoginManagerResponseDTO response = new LoginManagerResponseDTO
            {
                Token = session.Token
            };

            return response;
        }
    }
}
