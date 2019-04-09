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
        UserLoginManager _userLoginManager;
        UserManagementManager _userManagementManager;
        DatabaseContext _db;

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
                _db = new DatabaseContext();

                LoginManagerResponseDTO loginAttempt;
                loginAttempt = _userLoginManager.LoginFromSSO(
                    _db,
                    requestPayload.Email,
                    userSSOID,
                    requestPayload.Signature,
                    requestPayload.PreSignatureString());

                LoginResponseDTO response = new LoginResponseDTO
                {
                    redirectURL = "https://pointmap.net/#/login/?token=" + loginAttempt.Token
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return ResponseMessage(LoginErrorHandler.HandleDatabaseException(e, _db));
            }
        }
    }
}
