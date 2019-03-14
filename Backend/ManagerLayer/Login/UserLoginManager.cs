using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.AccessControl;
using ManagerLayer.Models;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
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

        public LoginManagerResponseDTO LoginFromSSO(DatabaseContext _db,  string username, Guid ssoID)
        {
            LoginManagerResponseDTO response;
            _userManagementManager = new UserManagementManager();
            var user = _userManagementManager.GetUserBySSOID(ssoID);
            // check if user does not exist
            if (user == null)
            {
                // create new user
                try
                {
                    user = _userManagementManager.CreateUser(_db, username, ssoID);
                }
                catch (InvalidEmailException ex)
                {
                    throw new InvalidEmailException(ex.Message);
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
                _db.Entry(user).State = System.Data.Entity.EntityState.Detached;
                _db.Entry(session).State = System.Data.Entity.EntityState.Detached;
                throw new InvalidDbOperationException();
            }
            response =  new LoginManagerResponseDTO
            {
                userid = user.Id,
                token = session.Token
            };
            return response;
        }
    }
}
