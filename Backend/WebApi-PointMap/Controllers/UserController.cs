using DataAccessLayer.Database;
using ManagerLayer.SSOUtility;
using DTO;
using System;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.Models;
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
                    var loginAttempt = _userLoginManager.LoginFromSSO(
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
    }
}
