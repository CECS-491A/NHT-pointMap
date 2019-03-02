using DataAccessLayer.Database;
using ManagerLayer.AccessControl;
using ManagerLayer.Login;
using ManagerLayer.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_PointMap.Models;

namespace WebApi_PointMap.Controllers
{
    public class ApplicationLaunchFromSS0Controller : ApiController
    {
        UserLoginManager _userLoginManager;

        // POST api/ssolaunch/user/login
        [HttpPost]
        [Route("api/sso/user/login")]
        public IHttpActionResult LoginFromSSO([FromBody] LoginPOCO value) //using a POCO to represent request
        {
            _userLoginManager = new UserLoginManager();
            ResponsePOCO response;
            if (value == null)
            {
                response = new ResponsePOCO { Data = null, Timestamp = DateTime.UtcNow };
                return Ok(response);
            }
            var loginAttempt = _userLoginManager.LoginFromSSO(value.Email, value.SSOUserId);
            if (loginAttempt.Data == null)
            {
                response = new ResponsePOCO
                {
                    Data = loginAttempt.Message,
                    Timestamp = DateTime.UtcNow
                };
                return Ok(response);
            }
            response = new ResponsePOCO
            {
                Data = loginAttempt.Data,
                Timestamp = DateTime.UtcNow
            };
            return Ok(response);
        }
    }
}
