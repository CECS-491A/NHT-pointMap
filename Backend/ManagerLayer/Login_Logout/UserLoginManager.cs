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

namespace ManagerLayer.Login_Logout
{
    public class UserLoginManager
    {
        UserManagementManager _userManagementManager;
        AuthorizationManager _authorizationManager;
        TokenService _tokenService;
        LogRequestDTO newLog;
        FormUrlEncodedContent logContent;
        LoggingManager loggingManager;

        public LoginManagerResponseDTO LoginFromSSO(
            DatabaseContext _db, string Username, Guid ssoID, string Signature, string PreSignatureString)
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
            var user = _userManagementManager.GetUser(ssoID);
            // check if user does not exist
            if (user == null)
            {
                // create new user
                try
                {
                    user = _userManagementManager.CreateUser(Username, ssoID);
                    _db.SaveChanges();
                    newLog = new LogRequestDTO(ssoID.ToString(), Username,
                        "Login/Registration API", user.Username, "Successful registration of new User", 
                        "Line 54 UserLoginManager in ManagerLayer\n" +
                        "Route Reference UserController in WebApi-PointMap");
                    loggingManager.sendLogSync(newLog);

                }
                catch (InvalidEmailException ex)
                {
                    throw new InvalidEmailException(ex.Message);
                }
                catch (InvalidDbOperationException)
                {
                    _db.Entry(user).State = System.Data.Entity.EntityState.Detached;
                    throw new InvalidDbOperationException("User was not created");
                }
            }
            _authorizationManager = new AuthorizationManager();
            Session session = _authorizationManager.CreateSession(_db, user);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception)
            {
                _db.Entry(session).State = System.Data.Entity.EntityState.Detached;
                throw new InvalidDbOperationException("Session was not created");
            }
            LoginManagerResponseDTO response;
            response = new LoginManagerResponseDTO
            {
                Token = session.Token
            };
            newLog = new LogRequestDTO(ssoID.ToString(), Username,
                        "Login/Registration API", user.Username, "Successful login of user",
                        "Line 85 UserLoginManager in ManagerLayer\n" +
                        "Route Reference UserController in WebApi-PointMap");
            loggingManager.sendLogSync(newLog);

            return response;
        }
    }
}
