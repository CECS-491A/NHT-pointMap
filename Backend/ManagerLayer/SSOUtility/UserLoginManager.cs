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
using ManagerLayer.Users;

namespace ManagerLayer.SSOUtility
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

        public Session LoginFromSSO(string Username, Guid ssoID, string Signature, string PreSignatureString)
        {
            ////////////////////////////////////////
            /// User oAuth at the indivudal application level
            // verify if the login payload is valid via its signature
            _tokenService = new TokenService();
            loggingManager = new LoggingManager();
            if (!_tokenService.isValidSignature(PreSignatureString, Signature))
            {
                newLog = new LogRequestDTO(ssoID.ToString(), Username,
                        "Login/Registration API", Username, "Invalid signing attempt",
                        "Line 35 UserLoginManager in ManagerLayer\n" +
                        "Route Reference UserController in WebApi-PointMap");
                loggingManager.sendLogSync(newLog);
                throw new InvalidTokenSignatureException("Session is not valid.");
            }
            ////////////////////////////////////////
            
            _userManagementManager = new UserManagementManager(_db);
            var _userManager = new UserManager(_db);
            var user = _userManagementManager.GetUser(ssoID);
            Session session;
            // check if user does not exist
            if (user == null)
            {
                // create new user, UserAlreadyExistsException thrown if user with email already exists
                session = _userManager.Register(Username, ssoID);
                return session;
            }
            session = _userManager.Login(user);
            return session;
        }
    }
}
