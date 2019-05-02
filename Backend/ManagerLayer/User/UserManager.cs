using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO;
using ManagerLayer.AccessControl;
using ManagerLayer.Logging;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.Users
{
    public class UserManager
    {
        DatabaseContext _db;
        LogRequestDTO newLog;
        LoggingManager _loggingManager;

        public UserManager(DatabaseContext db)
        {
            _db = db;
            _loggingManager = new LoggingManager();
        }

        public Session Login(User user)
        {
            var _authorizationManager = new AuthorizationManager(_db);
            Session session = _authorizationManager.CreateSession(user);
            newLog = new LogRequestDTO(user.Id.ToString(), user.Username,
                        "Login/Registration API", user.Username, "Successful login of user",
                        "Line 59 UserLoginManager in ManagerLayer\n" +
                        "Route Reference UserController in WebApi-PointMap");
            _loggingManager.sendLogSync(newLog);
            return session;
        }

        public Session Register(string Username, Guid ssoID)
        {
            var _userManagementManager = new UserManagementManager(_db);
            try
            {
                var user = _userManagementManager.CreateUser(Username, ssoID);
                newLog = new LogRequestDTO(ssoID.ToString(), Username,
                    "Login/Registration API", user.Username, "Successful registration of new User",
                    "Line 51 UserLoginManager in ManagerLayer\n" +
                    "Route Reference UserController in WebApi-PointMap");
                _loggingManager.sendLogSync(newLog);
                var _authorizationManager = new AuthorizationManager(_db);
                Session session = _authorizationManager.CreateSession(user);
                return session;
            }
            catch (UserAlreadyExistsException e)
            {
                newLog = new LogRequestDTO(ssoID.ToString(), Username,
                   "Login/Registration API", Username, "User with email already exists, create user prevented in registration.",
                   "Line 51 Register in UserManager\n" +
                   "Route Reference UserController in WebApi-PointMap");
                _loggingManager.sendLogSync(newLog);
                throw new UserAlreadyExistsException(e.Message);
            }
        }

        // Logout a user by deleteing their session, single session logout
        public void Logout(string token)
        {
            var _sessionService = new SessionService();
            var session = _sessionService.GetSession(_db, token);
            if (session == null)
            {
                return;
            }
            _sessionService.DeleteSession(_db, session.Token);
        }

        // Logout a user by their user ID, delete all sessions of that user
        public void Logout(User user)
        {
            var _sessionService = new SessionService();
            _sessionService.DeleteSessionsOfUser(_db, user.Id);
        }
    }
}
