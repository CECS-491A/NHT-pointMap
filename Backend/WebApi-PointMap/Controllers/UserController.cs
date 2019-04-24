using DataAccessLayer.Database;
using ManagerLayer.Login;
using DTO;
using System;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.Models;
using ManagerLayer.UserManagement;
using WebApi_PointMap.ErrorHandling;
using Logging.Logging;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {
        Logger logger;
        LogRequestDTO newLog;

        private Guid userSSOID;

        public UserController()
        {
            logger = new Logger();
            newLog = new LogRequestDTO();
        }

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
                    userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    var _userLoginManager = new UserLoginManager(_db);
                    LoginManagerResponseDTO loginAttempt;
                    loginAttempt = _userLoginManager.LoginFromSSO(
                        requestPayload.Email,
                        userSSOID,
                        requestPayload.Signature,
                        requestPayload.PreSignatureString());
                    _db.SaveChanges();

                    if (loginAttempt.newUser)
                    {
                        newLog = logger.initalizeAnalyticsLog("New user registration in UserController line 40\n" +
                            "Route: POST api/user/login", newLog.registrationSource);
                        newLog.ssoUserId = userSSOID.ToString();
                        newLog.email = requestPayload.Email;
                        logger.sendLogAsync(newLog);
                    }
                    else
                    {
                        newLog = logger.initalizeAnalyticsLog("Existing user login in UserController line 40\n" +
                            "Route: POST api/user/login", newLog.loginSource);
                        newLog.ssoUserId = userSSOID.ToString();
                        newLog.email = requestPayload.Email;
                        logger.sendLogAsync(newLog);
                    }

                    LoginResponseDTO response = new LoginResponseDTO
                    {
                        redirectURL = "https://pointmap.net/#/login/?token=" + loginAttempt.Token
                    };

                    return Ok(response);

                }
                catch (Exception e)
                {
                    logger.sendErrorLog(newLog.ssoSource, e.StackTrace, userSSOID.ToString(),
                    requestPayload.Email, null);

                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }
    }
}
