using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerLayer.UserManagement;
using ManagerLayer.AccessControl;
using ServiceLayer.Services;
using DTO;
using ManagerLayer.Logging;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.Login_Logout
{
    public class UserLogoutManager
    {
        UserManagementManager _userManagementManager;
        AuthorizationManager _authorizationManager;
        TokenService _tokenService;
        LoggingManager _loggingManager;
        LogRequestDTO newLog;
        SessionService _sessionService;

        public LogoutManagerResponseDTO LogoutFromSSO(DatabaseContext _db, string token)
        {

            _authorizationManager = new AuthorizationManager();
            _sessionService = new SessionService();


            Session session = _sessionService.GetSession(_db, token);
            
            try
            {
                session = _authorizationManager.ExpireSession(_db, token);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                _db.Entry(session).State = System.Data.Entity.EntityState.Detached;
                throw new InvalidDbOperationException("Session was not found");
            }

            LogoutManagerResponseDTO response;
            response = new LogoutManagerResponseDTO
            {
                Token = session.Token
            };


            newLog = new LogRequestDTO("", "", DateTime.UtcNow.ToString(),"","","" );
            _loggingManager.sendLogSync(newLog);
            return response;
        }
    }



}


