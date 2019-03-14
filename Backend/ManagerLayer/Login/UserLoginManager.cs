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

namespace ManagerLayer.Login
{
    public class UserLoginManager
    {
        UserManagementManager _userManagementManager;
        AuthorizationManager _authorizationManager;

        public LoginManagerResponseDTO LoginFromSSO(string username, Guid ssoID)
        {
            LoginManagerResponseDTO response;
            _userManagementManager = new UserManagementManager();
            _authorizationManager = new AuthorizationManager();
            var user = _userManagementManager.GetUserBySSOID(ssoID);
            // check if user does not exist
            if (user == null)
            {
                // create new user
                user = _userManagementManager.CreateUser(username, ssoID);
                if (user == null)
                {
                    response =  new LoginManagerResponseDTO
                    {
                        Data = null,
                        Message = "User can not be created",
                        Timestamp = DateTime.UtcNow
                    };
                    return response;
                }
            }
            string token = _authorizationManager.CreateSession(user);
            response =  new LoginManagerResponseDTO
            {
                Data = new {
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Email,
                        ssoID = user.SSOId
                    }
                },
                Message = "Login Successful",
                Timestamp = DateTime.UtcNow
            };
            return response;
        }
    }
}
