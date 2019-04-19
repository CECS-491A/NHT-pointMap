using DataAccessLayer.Database;
using ManagerLayer.Login;
using DTO;
using System;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.Models;
using ManagerLayer.UserManagement;
using WebApi_PointMap.ErrorHandling;
using ManagerLayer.Logging;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {
        UserLoginManager _userLoginManager;
        DatabaseContext _db;
        LoggingManager _loggingManager;
        LogRequestDTO newLog;

        public UserController()
        {
            _userLoginManager = new UserLoginManager();
            _loggingManager = new LoggingManager();
            _db = new DatabaseContext();
        }

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

                LoginManagerResponseDTO loginAttempt;
                loginAttempt = _userLoginManager.LoginFromSSO(
                    _db,
                    requestPayload.Email,
                    userSSOID,
                    requestPayload.Signature,
                    requestPayload.PreSignatureString());

                _db.SaveChanges();
                newLog = new LogRequestDTO(); //Creating Logging data transfer object
                newLog.email = requestPayload.Email;
                newLog.ssoUserId = userSSOID.ToString();
                newLog.source = "Login Controller";
                newLog.details = "Successful Login";
                _loggingManager.sendLogAsync(newLog); //Attempts to send log async

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
