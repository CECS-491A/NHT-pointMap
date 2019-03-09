using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLayer.Services;
using ManagerLayer.Models;
using ManagerLayer.UserManagement;
using ManagerLayer.AccessControl;
using ServiceLayer.Services;

namespace ManagerLayer.Login
{
    class UserLogoutManager
    {
        UserLogoutManager response;
        AuthorizationManager _authorizationManager;
        UserManagementManager _userManagementManager;

        public LogoutManagerResponsePOCO Logout(Guid id, string token)
        {
            LogoutManagerResponsePOCO response;

            var user = _userManagementManager.GetUser(id);
            
            
            var result = _authorizationManager.ExpireSession(token, id);

            if (result == 1)
            {
                response = new LogoutManagerResponsePOCO
                {
                    id = id,
                    message = "You have been logged out",
                    Timestamp = DateTime.UtcNow

                };
            }
            else
            {

                response = new LogoutManagerResponsePOCO
                {
                    id = id,
                    message = "Error: You have not been logged out",
                    Timestamp = DateTime.UtcNow

                };
            }
            return response;



        }
        public LogoutManagerResponsePOCO LogoutFromSSO(string ssoID)
        {
            return response = new LogoutManagerResponsePoco

        }
        

    }
}
