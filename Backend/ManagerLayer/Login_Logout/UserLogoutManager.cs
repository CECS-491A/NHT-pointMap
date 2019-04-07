using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerLayer.UserManagement;
using ManagerLayer.AccessControl;
using ServiceLayer.Services;
using DTO;
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

        public LogoutManagerResponseDTO LogoutFromSSO(DatabaseContext _db, string email, string token)
        {

            _authorizationManager = new AuthorizationManager();

            Guid userID = _userManagementManager.GetUser(email).Id;
            Session session = _authorizationManager.ExpireSession(_db, token);

            try
            {

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
            return response;
        }
    }



}


