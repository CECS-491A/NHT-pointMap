﻿using DataAccessLayer.Database;
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
        LogRequestDTO newLog;
        LoggingManager loggingManager;
        DatabaseContext _db;

        public UserLoginManager(DatabaseContext db)
        {
            _db = db;
        }

        public LoginManagerResponseDTO LoginFromSSO(string Username, Guid ssoID, long timestamp, string signature)
        {
            ////////////////////////////////////////
            /// User oAuth at the indivudal application level
            // verify if the login payload is valid via its signature
            var _ssoServiceAuth = new KFC_SSO_APIService.RequestPayloadAuthentication();
            loggingManager = new LoggingManager();
            if (!_ssoServiceAuth.IsValidClientRequest(ssoID, Username, timestamp, signature))
            {
                newLog = new LogRequestDTO(ssoID.ToString(), Username,
                        "Login/Registration API", Username, "Invalid signing attempt",
                        "Line 35 UserLoginManager in ManagerLayer\n" +
                        "Route Reference UserController in WebApi-PointMap");
                loggingManager.sendLogSync(newLog);
                throw new InvalidTokenSignatureException("Invalid token signature.");
            }
            ////////////////////////////////////////
            
            _userManagementManager = new UserManagementManager(_db);
            var user = _userManagementManager.GetUser(ssoID);
            // check if user does not exist
            if (user == null)
            {
                // create new user
                user = _userManagementManager.CreateUser(Username, ssoID);
                newLog = new LogRequestDTO(ssoID.ToString(), Username,
                    "Login/Registration API", user.Username, "Successful registration of new User", 
                    "Line 51 UserLoginManager in ManagerLayer\n" +
                    "Route Reference UserController in WebApi-PointMap");
                loggingManager.sendLogSync(newLog);
            }
            _authorizationManager = new AuthorizationManager(_db);
            Session session = _authorizationManager.CreateSession(user);

            LoginManagerResponseDTO response = new LoginManagerResponseDTO
            {
                Token = session.Token
            };
            newLog = new LogRequestDTO(ssoID.ToString(), Username,
                        "Login/Registration API", user.Username, "Successful login of user",
                        "Line 59 UserLoginManager in ManagerLayer\n" +
                        "Route Reference UserController in WebApi-PointMap");
            loggingManager.sendLogSync(newLog);

            return response;
        }
    }
}
