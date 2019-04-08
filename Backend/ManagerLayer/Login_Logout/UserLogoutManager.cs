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
        AuthorizationManager _authorizationManager;
        LoggingManager _loggingManager;
        LogRequestDTO newLog;

        public LogoutManagerResponseDTO LogoutFromSSO(DatabaseContext _db, string token)
        {

            _authorizationManager = new AuthorizationManager();
            _loggingManager = new LoggingManager();
            Session session = new Session();
            User user = new User();

            
            try
            {
                session = _authorizationManager.ExpireSession(_db, token);
                _db.SaveChanges();
                
            }
            catch (Exception e)
            {
                _db.Entry(session).State = System.Data.Entity.EntityState.Detached;
                throw new InvalidDbOperationException("Session could not be expired"+e.Message);
            }

            LogoutManagerResponseDTO response;
            if (session != null)
            {
                response = new LogoutManagerResponseDTO
                {
                    Token = session.Token
                };
                newLog = new LogRequestDTO(session.UserId.ToString(), session.UserId.ToString(),
                    "Logout/Login/Registration API", session.UserId.ToString(), "Succesful Logout and Session expiration of existing user",
                    "Line 36 UserLogoutManager in ManagerLayer\nSession token: " + session.Token);
                _loggingManager.sendLogSync(newLog);
                return response;

            }

            response = new LogoutManagerResponseDTO
            {
                Token = null
            };
            return response;






        }
    }



}


