using DataAccessLayer.Database;
using System;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.ErrorHandling;
using static ServiceLayer.Services.ExceptionService;
using ServiceLayer.Services;
using System.ComponentModel.DataAnnotations;
using ManagerLayer.KFC_SSO_Utility;
using System.Threading.Tasks;
using System.Net.Http;
using DTO.KFCSSO_API;
using ManagerLayer.UserManagement;
using ServiceLayer.KFC_API_Services;
using ManagerLayer.Users;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {

        /// <summary>
        /// SSO Service: User login via SSO App launch.
        /// </summary>
        /// <param name="requestPayload"></param>
        /// <returns> redirect to Pointmap landing page with url token param </returns>
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
                    Guid userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    var _ssoLoginManager = new KFC_SSO_Manager(_db);
                    // user will get logged in or registered
                    var loginSession = _ssoLoginManager.LoginFromSSO(
                        requestPayload.Email,
                        userSSOID,
                        requestPayload.Timestamp,
                        requestPayload.Signature);
                    _db.SaveChanges();
                    var redirectURL = "https://pointmap.net/#/login/?token=" + loginSession.Token;
                    var response = SSOLoginResponse.ResponseRedirect(Request, redirectURL);
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

        /// <summary>
        /// Pointmap Service: 
        /// 
        /// Delete request from user within Pointmap. Delete self from Pointmap and SSO.
        /// </summary>
        /// <returns> success of delete, 404 if user does not exist. </returns>
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
                    }
                    var _ssoAPIManager = new KFC_SSO_Manager(_db);
                    var requestSuccessful = await _ssoAPIManager.DeleteUserFromSSOviaPointmap(user);
                    if (requestSuccessful)
                    {
                        _userManager.DeleteUserAndSessions(user.Id);
                        _db.SaveChanges();
                        return Ok("User was deleted from Pointmap and SSO");
                    }
                    var response = Content(HttpStatusCode.InternalServerError, "User was not able to be deleted from SSO.");
                    return response;
                }
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
                    var response = ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                    return response;
                }
            }
        }

        /// <summary>
        /// Pointmap Service: 
        /// 
        /// User delete request to delete self from Pointmap
        /// </summary>
        /// <returns> status </returns>
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
                    var responseInternalError = ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                    return responseInternalError;
                }
            }
        }

        /// <summary>
        /// SSO Service: 
        /// 
        /// User delete request sent within SSO to delete self from SSO when 
        ///     deleted from all apps.
        /// </summary>
        /// <param name="requestPayload"></param>
        /// <returns> delete status </returns>
        [HttpPost]
        [Route("sso/user/delete")] // request from sso to delete user self from sso to all apps
        public IHttpActionResult DeleteViaSSO([FromBody, Required] LoginRequestPayload requestPayload)
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

        /// <summary>
        /// SSO Service: 
        /// 
        /// User request logout from within SSO, logout propagates to all apps.
        /// </summary>
        /// <param name="requestPayload"></param>
        /// <returns> logout status </returns>
        [HttpPost]
        [Route("sso/user/logout")]
        public IHttpActionResult LogoutViaSSO([FromBody] LogoutRequestPayload requestPayload)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    // throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);

                    // throws ExceptionService.InvalidGuidException
                    var userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    var _userManagementManager = new UserManagementManager(_db);
                    var user = _userManagementManager.GetUser(userSSOID);
                    if (user == null)
                    {
                        return Content(HttpStatusCode.NotFound, "User does not exist.");
                    }
                    var _userManager = new UserManager(_db);
                    _userManager.Logout(user);
                    _db.SaveChanges();
                    return Ok("User was logged out.");
                }
                catch (Exception e)
                {
                    if (e is InvalidTokenSignatureException)
                    {
                        return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                    }
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
