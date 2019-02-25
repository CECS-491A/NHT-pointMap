using DataAccessLayer.Database;
using ManagerLayer.AccessControl;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_PointMap.Models;

namespace WebApi_PointMap.Controllers
{
    public class SSOLaunchLoginController : ApiController
    {
        UserManagementManager _umm;
        AuthorizationManager _am;

        // POST api/ssolaunch/user/login
        [HttpPost]
        [Route("api/ssolaunch/user/login")]
        public IHttpActionResult Post([FromBody] LoginPOCO value) //using a POCO to represent request
        {
            _umm = new UserManagementManager();
            _am = new AuthorizationManager();
            ResponsePOCO response;
            if (value == null)
            {
                response = new ResponsePOCO { Data = null, Timestamp = DateTime.UtcNow };
                return Ok(response);
            }
            var user = _umm.GetUser(value.Email);
            if (user == null)
            {
                user = _umm.CreateUser(value.Email, value.Name, DateTime.UtcNow);
            }
            // check if user has existing session
            var existingSession = _am.ExistingSession(user);
            if (existingSession != null)
            {
                _am.DeleteSession(existingSession.Token, user.Id);
            }
            string token = _am.CreateSession(user);
            response = new ResponsePOCO{ Data = new { user = user, token = token }, Timestamp = DateTime.UtcNow };
            return Ok(response);
        }
    }
}
