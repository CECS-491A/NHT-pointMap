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
        IPasswordService _passwordService;
        UserManagementManager _userManagementManager;
        AuthorizationManager _authorizationManager;

        public LoginManagerResponsePOCO Login(string username, string password)
        {
            LoginManagerResponsePOCO response;
          
            _passwordService = new PasswordService();
            var user = _userManagementManager.GetUser(username);
            if (user == null)
            {
                response = new LoginManagerResponsePOCO
                {
                    Data = null,
                    Message = "User does not exist",
                    Timestamp = DateTime.UtcNow
                };
                return response;
            }
            var StoredPassword = user.PasswordHash;
            var AttemptedPassword = _passwordService.HashPassword(password, user.PasswordSalt);
            if (StoredPassword == AttemptedPassword)
            {
                string token = _authorizationManager.CreateSession(user);
                return new LoginManagerResponsePOCO
                {
                    Data = token,
                    Message = "Login Successful",
                    Timestamp = DateTime.UtcNow
                };
            }
            return new LoginManagerResponsePOCO
            {
                Data = null,
                Message = "Invalid password entered",
                Timestamp = DateTime.UtcNow
            };
        }

        public LoginManagerResponsePOCO LoginFromSSO(string username, string ssoID)
        {
            LoginManagerResponsePOCO response;
            _userManagementManager = new UserManagementManager();
            _authorizationManager = new AuthorizationManager();
            var user = _userManagementManager.GetUser(username);
            // check if user does not exist
            if (user == null)
            {
                // create new user
                user = _userManagementManager.CreateUser(username, ssoID, DateTime.UtcNow);
                if (user == null)
                {
                    response =  new LoginManagerResponsePOCO
                    {
                        Data = null,
                        Message = "User can not be created",
                        Timestamp = DateTime.UtcNow
                    };
                    return response;
                }
            }
            string token = _authorizationManager.CreateSession(user);
            response =  new LoginManagerResponsePOCO
            {
                Data = new {
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        username = username,
                        ssoID = ssoID
                    }
                },
                Message = "Login Successful",
                Timestamp = DateTime.UtcNow
            };
            return response;
        }
    }
}
