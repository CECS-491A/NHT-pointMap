using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO;
using ManagerLayer.AccessControl;
using ManagerLayer.Logging;
using ManagerLayer.UserManagement;
using ManagerLayer.Users;
using ServiceLayer.KFC_API_Services;
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
        LogRequestDTO newLog;
        LoggingManager loggingManager;
        DatabaseContext _db;
        SignatureService _ssoServiceAuth;

        public KFC_SSO_Manager(DatabaseContext db)
        {
            _db = db;
            _ssoServiceAuth = new SignatureService();
        }

        public Session LoginFromSSO(string Username, Guid ssoID, long timestamp, string signature)
        {
            ////////////////////////////////////////
            /// User oAuth at the indivudal application level
            // verify if the login payload is valid via its signature
            loggingManager = new LoggingManager();
            if (!_ssoServiceAuth.IsValidClientRequest(ssoID.ToString(), Username, timestamp, signature))
            {
                newLog = new LogRequestDTO(ssoID.ToString(), Username,
                        "Login/Registration API", Username, "Invalid signing attempt",
                        "Line 35 UserLoginManager in ManagerLayer\n" +
                        "Route Reference UserController in WebApi-PointMap");
                loggingManager.sendLogSync(newLog);
                throw new InvalidTokenSignatureException("Session is not valid.");
            }
            ////////////////////////////////////////
            try
            {
                new System.Net.Mail.MailAddress(Username);
                _userManagementManager = new UserManagementManager(_db);
                var _userManager = new UserManager(_db);
                var user = _userManagementManager.GetUser(ssoID);
                Session session;
                if (user == null)
                {
                    // check if user does not exist
                    // create new user, UserAlreadyExistsException thrown if user with email already exists
                    session = _userManager.Register(Username, ssoID);
                    return session;
                }
                session = _userManager.Login(user);
                return session;
            }
            catch (FormatException)
            {
                throw new InvalidEmailException("Invalid email format.");
            }
        }

        public void LogoutFromSSO(string Username, Guid ssoID, long timestamp, string signature)
        {
            // Check if valid signature request
            if (!_ssoServiceAuth.IsValidClientRequest(ssoID.ToString(), Username, timestamp, signature))
            {
                newLog = new LogRequestDTO(ssoID.ToString(), Username,
                       "Logout/SSO API", Username, "Invalid logout attempt",
                       "Line 79 KFC_SSO_Manager in ManagerLayer\n" +
                       "Route Reference UserController in WebApi-PointMap");
                throw new InvalidTokenSignatureException("Session is not valid.");
            }
            _userManagementManager = new UserManagementManager(_db);
            var _userManager = new UserManager(_db);
            var user = _userManagementManager.GetUser(ssoID);
            if (user == null)
            {
                return;
            }
            // Delete all sessions of the user
            _userManager.Logout(user);
        }

        public async Task<bool> DeleteUserFromSSOviaPointmap(User user)
        {
            var _ssoAPI = new SSO_APIService();
            var requestResponse = await _ssoAPI.DeleteUserFromSSO(user);
            if (requestResponse.IsSuccessStatusCode)
            {
                return true;
            }
            // throw response of request
            throw new KFCSSOAPIRequestException(requestResponse.Content.ToString());
        }
    }
}
