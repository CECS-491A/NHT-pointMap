using DataAccessLayer.Database;
using ManagerLayer.Login;
using ManagerLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_PointMap.Models;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {
        UserLoginManager _userLoginManager;

        // POST api/ssolaunch/user/login
        [HttpPost]
        [Route("api/sso/user/login")]
        public IHttpActionResult LoginFromSSO([FromBody] LoginDTO request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Content((HttpStatusCode)412, ModelState);
            }
            _userLoginManager = new UserLoginManager();
            Guid userSSOID;
            try
            {
                // check if valid SSO ID format
                userSSOID = Guid.Parse(request.SSOUserId);
            }
            catch (Exception)
            {
                return Content((HttpStatusCode)400, "Invalid SSO ID");
            }
            using (var _db = new DatabaseContext())
            {
                LoginManagerResponseDTO loginAttempt;
                try
                {
                    loginAttempt = _userLoginManager.LoginFromSSO(_db, request.Email, userSSOID);
                }
                catch (InvalidDbOperationException ex)
                {
                    return Content((HttpStatusCode)500, ex.Message);
                }
                catch (InvalidEmailException ex)
                {
                    return Content((HttpStatusCode)400, ex.Message);
                }
                LoginResponseDTO response = new LoginResponseDTO
                {
                    token = loginAttempt.token,
                    userId = loginAttempt.userid
                };
                return Ok(response);
            }
        }
    }
}
