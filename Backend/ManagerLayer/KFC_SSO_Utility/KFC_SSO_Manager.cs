using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO;
using ManagerLayer.AccessControl;
using ManagerLayer.Logging;
using ManagerLayer.UserManagement;
using ManagerLayer.Users;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.KFC_SSO_Utility
{
    public class KFC_SSO_Manager
    {
        UserManagementManager _userManagementManager;
        AuthorizationManager _authorizationManager;
        LogRequestDTO newLog;
        LoggingManager loggingManager;
        DatabaseContext _db;

        public KFC_SSO_Manager(DatabaseContext db)
        {
            _db = db;
        }

        public Session LoginFromSSO(string Username, Guid ssoID, string Signature, string PreSignatureString)
        {
            ////////////////////////////////////////
            /// User oAuth at the indivudal application level
            // verify if the login payload is valid via its signature
            var _ssoServiceAuth = new KFC_SSO_APIService.RequestPayloadAuthentication();
            loggingManager = new LoggingManager();
            if (!_ssoServiceAuth.IsValidClientRequest(PreSignatureString, Signature))
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

        public static bool DeleteUserFromSSOviaPointmap(User user)
        {
            var _ssoAPI = new KFC_SSO_APIService();
            var requestResponse = _ssoAPI.DeleteUserFromSSO(user);
            if (requestResponse.IsSuccessStatusCode)
            {
                return true;
            }
            // throw response of request
            throw new KFCSSOAPIRequestException(requestResponse.Content.ToString());
        }
    }
}
