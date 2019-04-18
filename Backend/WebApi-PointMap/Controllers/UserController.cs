using DataAccessLayer.Database;
using ManagerLayer.Login;
using DTO;
using System;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.Models;
using ManagerLayer.UserManagement;
using WebApi_PointMap.ErrorHandling;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {

        // POST api/user/login
        [HttpPost]
        [Route("api/user/login")]
        public IHttpActionResult LoginFromSSO([FromBody] LoginDTO requestPayload)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);

                    //throws ExceptionService.InvalidGuidException
                    var userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    var _userLoginManager = new UserLoginManager(_db);
                    LoginManagerResponseDTO loginAttempt;
                    loginAttempt = _userLoginManager.LoginFromSSO(
                        requestPayload.Email,
                        userSSOID,
                        requestPayload.Signature,
                        requestPayload.PreSignatureString());

                    _db.SaveChanges();

                    LoginResponseDTO response = new LoginResponseDTO
                    {
                        redirectURL = "https://pointmap.net/#/login/?token=" + loginAttempt.Token
                    };

                    return Ok(response);

                }
                catch (Exception e)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
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
