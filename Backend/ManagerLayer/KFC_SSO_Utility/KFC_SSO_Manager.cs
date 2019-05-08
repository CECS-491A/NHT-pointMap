using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO;
using ManagerLayer.UserManagement;
using ManagerLayer.Users;
using ServiceLayer.KFC_API_Services;
using ServiceLayer.Services;
using System;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.KFC_SSO_Utility
{
    public class KFC_SSO_Manager
    {
        private UserManagementManager _userManagementManager;
        private readonly DatabaseContext _db;
        private SignatureService _ssoServiceAuth;
        private UserService _userService;

        public KFC_SSO_Manager(DatabaseContext db)
        {
            _db = db;
            _ssoServiceAuth = new SignatureService();
            _userService = new UserService(_db);
        }

        public async Task<Session> LoginFromSSO(string Username, Guid ssoID, long timestamp, string signature)
        {
            ////////////////////////////////////////
            /// User oAuth at the indivudal application level
            // verify if the login payload is valid via its signature
            var validSignature = _ssoServiceAuth.IsValidClientRequest(ssoID.ToString(), Username, timestamp, signature);
            if (!validSignature)
            {
                throw new InvalidTokenSignatureException("Session is not valid.");
            }
            ////////////////////////////////////////
            try
            {
                new System.Net.Mail.MailAddress(Username);
                var _userManager = new UserManager(_db);
                var user = _userService.GetUser(ssoID);
                Session session;
                if (user == null)
                {
                    // check if user does not exist
                    // create new user, UserAlreadyExistsException thrown if user with email already exists
                    session = await _userManager.Register(Username, ssoID);
                    return session;
                }
                session = await _userManager.Login(user);
                return session;
            }
            catch (FormatException)
            {
                throw new InvalidEmailException("Invalid email format.");
            }
        }

        public async void LogoutFromSSO(string Username, Guid ssoID, long timestamp, string signature)
        {
            // Check if valid signature request
            var validSignature = _ssoServiceAuth.IsValidClientRequest(ssoID.ToString(), Username, timestamp, signature);
            if (!validSignature)
            {
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
