using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO;
using ManagerLayer.AccessControl;
using ManagerLayer.Logging;
using ManagerLayer.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.Login
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

            LoginManagerResponseDTO response = new LoginManagerResponseDTO
            {
                Token = session.Token
            };
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
    }
}
