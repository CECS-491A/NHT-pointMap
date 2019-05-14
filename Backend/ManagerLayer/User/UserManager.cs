using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO;
using ManagerLayer.AccessControl;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
using System;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.Users
{
    public class UserManager
    {
        DatabaseContext _db;

        public UserManager(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Session> Login(User user)
        {
            var _authorizationManager = new AuthorizationManager(_db);
            Session session = _authorizationManager.CreateSession(user);
            return session;
        }

        public async Task<Session> Register(string Username, Guid ssoID)
        {
            var _userManagementManager = new UserManagementManager(_db);
            try
            {
                var user = _userManagementManager.CreateUser(Username, ssoID);
                var _authorizationManager = new AuthorizationManager(_db);
                Session session = _authorizationManager.CreateSession(user);
                return session;
            }
            catch (UserAlreadyExistsException e)
            {
                throw new UserAlreadyExistsException(e.Message);
            }
        }

        // Logout a user by deleteing their session, single session logout
        public void Logout(string token)
        {
            var _sessionService = new SessionService(_db);
            var session = _sessionService.ValidateSession(token);
            if (session == null)
            {
                return;
            }
            _sessionService.DeleteSession(session.Token);
        }

        // Logout a user by their user ID, delete all sessions of that user
        public void Logout(User user)
        {
            var _sessionService = new SessionService(_db);
            _sessionService.DeleteSessionsOfUser(user.Id);
        }
    }
}
