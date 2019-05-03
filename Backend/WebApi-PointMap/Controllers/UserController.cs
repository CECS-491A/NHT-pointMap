using DataAccessLayer.Database;
using System;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.ErrorHandling;
<<<<<<< HEAD
using Logging.Logging;
using System.Collections.Generic;
=======
>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
using static ServiceLayer.Services.ExceptionService;
using ServiceLayer.Services;
using System.ComponentModel.DataAnnotations;
using ManagerLayer.KFC_SSO_Utility;
using System.Threading.Tasks;
using System.Net.Http;
using DTO.KFCSSO_API;
using ManagerLayer.UserManagement;
using ServiceLayer.KFC_API_Services;

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
        public HttpResponseMessage LoginFromSSO([FromBody] LoginRequestPayload requestPayload)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);

                    //throws ExceptionService.InvalidGuidException
                    userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    var _ssoLoginManager = new KFC_SSO_Manager(_db);
                    // user will get logged in or registered
                    var loginSession = _ssoLoginManager.LoginFromSSO(
                        requestPayload.Email,
                        userSSOID,
<<<<<<< HEAD
                        requestPayload.Signature,
                        requestPayload.PreSignatureString());
=======
                        requestPayload.Timestamp,
                        requestPayload.Signature);

>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
                    _db.SaveChanges();

                    var redirectURL = "https://pointmap.net/#/login/?token=" + loginSession.Token;
                    var response = SSOLoginResponse.ResponseRedirect(this, redirectURL);
                    return response;
                }
                catch (Exception e)
                {
                    var response = new HttpResponseMessage();
                    if (e is InvalidTokenSignatureException)
                    {
                        response = AuthorizationErrorHandler.HandleException(e);
                        return response;
                    }
                    if (e is InvalidGuidException || e is InvalidEmailException)
                    {
                        response = GeneralErrorHandler.HandleException(e);
                        return response;
                    }
                    response = DatabaseErrorHandler.HandleException(e, _db);
                    return response;
                }
            }
        }

        // user delete self from pointmap and sso
        [HttpDelete]
        [Route("api/user/deletefromsso")]
        public async Task<IHttpActionResult> DeleteFromSSO()
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.NoTokenProvidedException
                    var token = ControllerHelpers.GetToken(Request);

                    //throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user == null)
                    {
                        return Content(HttpStatusCode.NotFound, "User does not exists.");
<<<<<<< HEAD
                    }
                    var _ssoAPIManager = new KFC_SSO_Manager();
                    var requestSuccessful = _ssoAPIManager.DeleteUserFromSSOviaPointmap(user);
=======
                    }
                    var _ssoAPIManager = new KFC_SSO_Manager();
                    var requestSuccessful = await _ssoAPIManager.DeleteUserFromSSOviaPointmap(user);
>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
                    if (requestSuccessful)
                    {
                        _userManager.DeleteUserAndSessions(user.Id);
                        _db.SaveChanges();
                        return Ok("User was deleted");
                    }
<<<<<<< HEAD
                    return Content(HttpStatusCode.InternalServerError, "User was not deleted.");
                }
=======
                    var response = Content(HttpStatusCode.InternalServerError, "User was not deleted.");
                    return response;
                }
>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
                catch (KFCSSOAPIRequestException ex)
                {
                    var response = Content(HttpStatusCode.ServiceUnavailable, ex.Message);
                    return response;
                }
                catch (UserNotFoundException e)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e)
                {
                    if (e is SessionNotFoundException || e is NoTokenProvidedException)
                    {
                        var responseAuthError = ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                        return responseAuthError;
                    }
<<<<<<< HEAD
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
=======
                    var response = ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                    return response;
                }
>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
            }
        }

        [HttpDelete]
        [Route("api/user/delete")]
        public IHttpActionResult Delete() // user delete self from pointmap
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.NoTokenProvidedException
                    var token = ControllerHelpers.GetToken(Request);

                    //throws ExceptionService.SessionNotFoundException
<<<<<<< HEAD
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user == null)
                    {
                        return Content(HttpStatusCode.NotFound, "User does not exists.");
                    }
                    //delete user self and their sessions
                    _userManager.DeleteUserAndSessions(user.Id);
                    _db.SaveChanges();
                    return Content(HttpStatusCode.OK, "User was deleted from Pointmap.");
                }
=======
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    var _userManager = new UserManagementManager(_db);
                    // throw exception if user not found
                    var user = _userManager.GetUser(session.UserId);
                    //delete user self and their sessions
                    _userManager.DeleteUserAndSessions(user.Id);
                    _db.SaveChanges();
                    var response = Content(HttpStatusCode.OK, "User was deleted from Pointmap.");
                    return response;
                }
>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
                catch (KFCSSOAPIRequestException ex)
                {
                    var responseAPIError = Content(HttpStatusCode.ServiceUnavailable, ex.Message);
                    return responseAPIError;
                }
                catch (UserNotFoundException e)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e)
                {
                    if (e is SessionNotFoundException || e is NoTokenProvidedException)
                    {
                        var responseAuthError = ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                        return responseAuthError;
                    }
<<<<<<< HEAD
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }

        [HttpPost]
        [Route("sso/user/delete")] // request from sso to delete user self from sso to all apps
        public IHttpActionResult DeleteViaSSO([FromBody, Required] LoginDTO requestPayload)
=======
                    var responseInternalError = ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                    return responseInternalError;
                }
            }
        }

        [HttpPost]
        [Route("sso/user/delete")] // request from sso to delete user self from sso to all apps
        public IHttpActionResult DeleteViaSSO([FromBody, Required] LoginRequestPayload requestPayload)
>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);

                    //throws ExceptionService.InvalidGuidException
                    var userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    // check valid signature
                    var _ssoServiceAuth = new SignatureService();
                    if (!_ssoServiceAuth.IsValidClientRequest(userSSOID.ToString(), requestPayload.Email, requestPayload.Timestamp, requestPayload.Signature))
                    {
                        throw new InvalidTokenSignatureException("Session is not valid.");
                    }

                    var _userManagementManager = new UserManagementManager(_db);
                    // throw exception if user does not exist
                    var user = _userManagementManager.GetUser(userSSOID);
                    _userManagementManager.DeleteUserAndSessions(userSSOID);
                    _db.SaveChanges();
                    return Ok("User was deleted");
                }
                catch (InvalidTokenSignatureException e)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (UserNotFoundException e)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e)
                {
                    if (e is InvalidModelPayloadException || e is InvalidGuidException)
                    {
                        return ResponseMessage(GeneralErrorHandler.HandleException(e));
                    }
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }
    }
}
