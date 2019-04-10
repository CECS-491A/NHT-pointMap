using DataAccessLayer.Database;
using ManagerLayer.Login;
using DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_PointMap.Models;
using System.Web.Http.Controllers;
using static ServiceLayer.Services.ExceptionService;
using ManagerLayer.UserManagement;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ServiceLayer.Services;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {
        UserLoginManager _userLoginManager;

        // POST api/user/login
        [HttpPost]
        [Route("api/user/login")]
        public IHttpActionResult LoginFromSSO([FromBody] LoginDTO requestPayload)
        {
            if (!ModelState.IsValid || requestPayload == null)
            {
                return Content((HttpStatusCode)412, ModelState);
            }
            _userLoginManager = new UserLoginManager();
            Guid userSSOID;
            try
            {
                // check if valid SSO ID format
                userSSOID = Guid.Parse(requestPayload.SSOUserId);
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
                    loginAttempt = _userLoginManager.LoginFromSSO(
                        _db,
                        requestPayload.Email,
                        userSSOID,
                        requestPayload.Signature,
                        requestPayload.PreSignatureString());
                }
                catch (InvalidTokenSignatureException ex)
                {
                    return Content((HttpStatusCode)401, ex.Message);
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
                    redirectURL = "https://pointmap.net/#/login/?token=" + loginAttempt.Token
                };

                return Ok(response);
            }
        }

        // delete user from sso and all apps
        [HttpDelete]
        [Route("api/user/deletefromsso")]
        public IHttpActionResult DeleteFromSSO()
        {
            var token = GetHeader(Request, "Token");
            if (token.Length < 1)
            {
                return Content(HttpStatusCode.Unauthorized, "No token provided.");
            }
            using (var _db = new DatabaseContext())
            {
                try
                {
                    var _sessionService = new SessionService();
                    var session = _sessionService.ValidateSession(_db, token);
                    if (session == null)
                    {
                        return Content(HttpStatusCode.NotFound, "Session is no longer available.");
                    }
                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user == null)
                    {
                        return Content(HttpStatusCode.NotFound, "User does not exists.");
                    }
                    var _ssoAPI = new KFC_SSO_APIService();
                    var requestResponse = _ssoAPI.DeleteUserFromSSO(user);
                    if (requestResponse.IsSuccessStatusCode)
                    {
                        return Ok("User was deleted");
                    }
                    return Content(HttpStatusCode.InternalServerError, "User was not delete.");
                }
                catch (KFCSSOAPIRequestException ex)
                {
                    return Content(HttpStatusCode.ServiceUnavailable, ex.Message);
                }
                catch (Exception ex)
                {
                    return Content((HttpStatusCode)500, ex.Message);
                }
            }
        }

        public string GetHeader(object request, string header)
        {
            IEnumerable<string> headerValues;
            var nameFilter = string.Empty;
            if (Request.Headers.TryGetValues(header, out headerValues))
            {
                nameFilter = headerValues.FirstOrDefault();
            }
            return nameFilter;
        }

        class UserRequestDTO
        {
            [Required]
            public string id { get; set; }
        }
    }
}
