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

        public LogoutManagerResponsePOCO Logout( string token)
        {
            LogoutManagerResponsePOCO response;
            
            
            var result = _authorizationManager.ExpireSession(token);

            if (result == 1)
            {
                response = new LogoutManagerResponsePOCO
                {
                    data = token,
                    message = "You have been logged out",
                    Timestamp = DateTime.UtcNow

                };
            }
            else
            {

                response = new LogoutManagerResponsePOCO
                {
                    data = token,
                    message = "Error: You have not been logged out",
                    Timestamp = DateTime.UtcNow

                };
            }
            return response;



        }
        public LogoutManagerResponsePOCO LogoutFromSSO(string token, string username)
        {
            LogoutManagerResponsePOCO response;
            _authorizationManager = new AuthorizationManager();
           


            var session = _authorizationManager.ExpireSession(token);

            if (session == null)
            {
                response = new LogoutManagerResponsePOCO
                {
                    data = token,
                    message = "No session was found.",
                    Timestamp = DateTime.UtcNow
                };
                return response;
            }
            response = new LogoutManagerResponsePOCO
            {
                data = new { token = token },
                message = "User has been logged out.",
                Timestamp = DateTime.UtcNow

            };
            return response;


           
        }
        

    }
}
