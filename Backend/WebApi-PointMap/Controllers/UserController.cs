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
        DatabaseContext _db;

        // POST api/user/login
        [HttpPost]
        [Route("api/user/login")]
        public IHttpActionResult LoginFromSSO([FromBody] LoginDTO requestPayload)
        {
            try
            {
                //throws ExceptionService.InvalidModelPayloadException
                ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);
                
                //throws ExceptionService.InvalidGuidException
                var userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);
                _db = new DatabaseContext();
                _userLoginManager = new UserLoginManager();

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
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }
    }
}
