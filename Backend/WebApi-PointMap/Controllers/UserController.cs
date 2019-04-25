using DataAccessLayer.Database;
using ManagerLayer.Login;
using DTO;
using System;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.Models;
using ManagerLayer.UserManagement;
using WebApi_PointMap.ErrorHandling;
using System.Collections.Generic;
using static ServiceLayer.Services.ExceptionService;
using ServiceLayer.Services;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using ManagerLayer.KFC_SSO_Utility;
using System.Threading.Tasks;

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
                    //throws invalid token signature exception
                    LoginManagerResponseDTO loginAttempt;
                    loginAttempt = _userLoginManager.LoginFromSSO(
                        requestPayload.Email,
                        userSSOID,
                        requestPayload.Timestamp,
                        requestPayload.Signature);

                    _db.SaveChanges();

                    var redirectURL = "https://pointmap.net/#/login/?token=" + loginAttempt.Token;

                    return Content(HttpStatusCode.TemporaryRedirect, redirectURL);

                }
                catch (InvalidTokenSignatureException e)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception e)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
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
                    }
                    var _ssoAPIManager = new KFC_SSO_Manager();
                    var requestSuccessful = await _ssoAPIManager.DeleteUserFromSSOviaPointmap(user);
                    if (requestSuccessful)
                    {
                        _userManager.DeleteUserAndSessions(user.Id);
                        _db.SaveChanges();
                        return Ok("User was deleted");
                    }
                    return Content(HttpStatusCode.InternalServerError, "User was not deleted.");
                }
                catch (KFCSSOAPIRequestException ex)
                {
                    return Content(HttpStatusCode.ServiceUnavailable, ex.Message);
                }
                catch (Exception e)
                {
                    if (e is SessionNotFoundException || e is NoTokenProvidedException)
                    {
                        return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                    }
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
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
                catch (KFCSSOAPIRequestException ex)
                {
                    return Content(HttpStatusCode.ServiceUnavailable, ex.Message);
                }
                catch (Exception e)
                {
                    if (e is SessionNotFoundException || e is NoTokenProvidedException)
                    {
                        return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                    }
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }

        [HttpPost]
        [Route("sso/user/delete")] // request from sso to delete user self from sso to all apps
        public IHttpActionResult DeleteViaSSO([FromBody, Required] LoginDTO requestPayload)
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
                    var _ssoServiceAuth = new KFC_SSO_APIService.RequestPayloadAuthentication();
                    if (!_ssoServiceAuth.IsValidClientRequest(userSSOID, requestPayload.Email, requestPayload.Timestamp, requestPayload.Signature))
                    {
                        throw new InvalidTokenSignatureException("Session is not valid.");
                    }

                    var _userManagementManager = new UserManagementManager(_db);
                    var user = _userManagementManager.GetUser(userSSOID);
                    if (user == null)
                    {
                        return Ok("User was never registered.");
                    }
                    _userManagementManager.DeleteUserAndSessions(userSSOID);
                    _db.SaveChanges();
                    return Ok("User was deleted");
                }
                catch (InvalidTokenSignatureException e)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
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
