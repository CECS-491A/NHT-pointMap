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
                        requestPayload.Signature,
                        requestPayload.PreSignatureString());

                    _db.SaveChanges();

                    LoginResponseDTO response = new LoginResponseDTO
                    {
                        redirectURL = "https://pointmap.net/#/login/?token=" + loginAttempt.Token
                    };

                    return Ok(response);

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
        public IHttpActionResult DeleteFromSSO()
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
                    var _ssoAPI = new KFC_SSO_APIService();
                    var requestResponse = _ssoAPI.DeleteUserFromSSO(user);
                    if (requestResponse.IsSuccessStatusCode)
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
                    return Content(HttpStatusCode.InternalServerError, "User was not delete.");
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
        public IHttpActionResult DeleteUser([FromBody, Required] LoginDTO requestPayload)
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
                    if (!_ssoServiceAuth.IsValidClientRequest(requestPayload.PreSignatureString(), requestPayload.Signature))
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
