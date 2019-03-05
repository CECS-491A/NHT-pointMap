using ManagerLayer.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_PointMap.Models;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {
        UserLoginManager _userLoginManager;

        // POST api/ssolaunch/user/login
        [HttpPost]
        [Route("api/sso/user/login")]
        public IHttpActionResult LoginFromSSO([FromBody] LoginDTO payload) //using a POCO to represent request
        {
            _userLoginManager = new UserLoginManager();
            ResponseDTO response;
            if (payload == null)
            {
                response = new ResponseDTO { Data = null, Timestamp = DateTime.UtcNow };
                return Ok(response);
            }
            Guid userSSOID;
            try
            {
                userSSOID = Guid.Parse(payload.SSOUserId);
            }
            catch (Exception)
            {
                response = new ResponseDTO
                {
                    Data = new {
                        message = "Invalid format for SSO ID"
                    },
                    Timestamp = DateTime.UtcNow
                };
                return Ok(response);
            }
            var loginAttempt = _userLoginManager.LoginFromSSO(payload.Email, userSSOID);
            if (loginAttempt.Data == null)
            {
                response = new ResponseDTO
                {
                    Data = loginAttempt.Message,
                    Timestamp = DateTime.UtcNow
                };
                return Ok(response);
            }
            response = new ResponseDTO
            {
                Data = loginAttempt.Data,
                Timestamp = DateTime.UtcNow
            };
            return Ok(response);
        }
    }
}
