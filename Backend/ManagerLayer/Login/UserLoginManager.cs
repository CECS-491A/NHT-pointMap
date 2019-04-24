using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.AccessControl;
using DTO;
using ManagerLayer.UserManagement;
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
        DatabaseContext _db;
        LoginManagerResponseDTO response;

        public UserLoginManager(DatabaseContext db)
        {
            _db = db;
            response = new LoginManagerResponseDTO();
        }

        public LoginManagerResponseDTO LoginFromSSO(string Username, Guid ssoID, string Signature, string PreSignatureString)
        {
            ////////////////////////////////////////
            /// User oAuth at the indivudal application level
            // verify if the login payload is valid via its signature
            var _ssoServiceAuth = new KFC_SSO_APIService.RequestPayloadAuthentication();
            if (!_ssoServiceAuth.IsValidClientRequest(PreSignatureString, Signature))
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
                user = _userManagementManager.CreateUser(Username, ssoID);
                response.newUser = true;
            }
            _authorizationManager = new AuthorizationManager(_db);
            Session session = _authorizationManager.CreateSession(user);

            
            response.Token = session.Token;

            return response;
        }
    }
}
