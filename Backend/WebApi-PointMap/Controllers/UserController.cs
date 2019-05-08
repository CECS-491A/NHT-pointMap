using DataAccessLayer.Database;
using System;
using System.Net;
using System.Web.Http;
using static ServiceLayer.Services.ExceptionService;
using System.ComponentModel.DataAnnotations;
using ManagerLayer.KFC_SSO_Utility;
using System.Threading.Tasks;
using System.Net.Http;
using DTO.KFCSSO_API;
using ManagerLayer.UserManagement;
using ServiceLayer.KFC_API_Services;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
        public async Task<HttpResponseMessage> LoginFromSSO([FromBody] LoginRequestPayload requestPayload)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    // Throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);

                    // Throws ExceptionService.InvalidGuidException
                    Guid userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    var _ssoLoginManager = new KFC_SSO_Manager(_db);
                    // user will get logged in or registered
                    var loginSession = await _ssoLoginManager.LoginFromSSO(
                        requestPayload.Email,
                        userSSOID,
                        requestPayload.Timestamp,
                        requestPayload.Signature);
                    _db.SaveChanges();
                    var redirectURL = "https://pointmap.net/#/login/?token=" + loginSession.Token;
                    var response = SSOLoginResponse.ResponseRedirect(Request, redirectURL);
                    return response;
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is InvalidModelPayloadException ||
                                            e is InvalidEmailException)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    response.Content = new StringContent(e.Message);
                    return response;
                }
                catch (Exception e) when (e is UserAlreadyExistsException)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent(e.Message);
                    return response;
                }
                catch (Exception e) when (e is InvalidTokenSignatureException)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent(e.Message);
                    return response;
                }
                catch (Exception e)
                {
                    if(e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
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
                    // Throws ExceptionService.NoTokenProvidedException
                    // Throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user == null)
                    {
                        return Ok();
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
                catch (Exception e) when (e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e) when (e is KFCSSOAPIRequestException)
                {
                    return Content(HttpStatusCode.ServiceUnavailable, e.Message);
                }
                catch (Exception e)
                {
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return Content(HttpStatusCode.InternalServerError, e.Message);
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
        public IHttpActionResult Delete() // User delete self from pointmap
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    // Throws ExceptionService.NoTokenProvidedException
                    // Throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    var _userManager = new UserManagementManager(_db);
                    // Throw exception if user not found
                    var user = _userManager.GetUser(session.UserId);
                    if (user == null)
                    {
                        return Content(HttpStatusCode.NotFound, "User does not exist.");
                    }
                    // Delete user self and their sessions
                    _userManager.DeleteUserAndSessions(user.Id);
                    _db.SaveChanges();
                    var response = Content(HttpStatusCode.OK, "User was deleted from Pointmap.");
                    return response;
                }
                catch (Exception e) when (e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e)
                {
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return Content(HttpStatusCode.InternalServerError, e.Message);
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
        [Route("sso/user/delete")] // Request from sso to delete user self from sso to all apps
        public IHttpActionResult DeleteViaSSO([FromBody, Required] LoginRequestPayload requestPayload)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    // Throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);

                    // Throws ExceptionService.InvalidGuidException
                    var userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                    // Check valid signature
                    var _ssoServiceAuth = new SignatureService();
                    var validSignature = _ssoServiceAuth.IsValidClientRequest(userSSOID.ToString(), requestPayload.Email, requestPayload.Timestamp, requestPayload.Signature);
                    if (!validSignature)
                    {
                        return Content(HttpStatusCode.Unauthorized, "Invalid Token signature.");
                    }

                    var _userManagementManager = new UserManagementManager(_db);
                    // Throw exception if user does not exist
                    var user = _userManagementManager.GetUser(userSSOID);
                    if (user == null)
                    {
                        return Content(HttpStatusCode.OK, "User does not exist");
                    }
                    _userManagementManager.DeleteUserAndSessions(userSSOID);
                    _db.SaveChanges();
                    return Ok("User was deleted");
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is InvalidModelPayloadException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e)
                {
                    if (e is DbUpdateException ||
                         e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return Content(HttpStatusCode.InternalServerError, e.Message);
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
                        return Content(HttpStatusCode.OK, "User does not exist.");
                    }
                    var _userManager = new UserManager(_db);
                    _userManager.Logout(user);
                    _db.SaveChanges();
                    return Ok("User was logged out.");
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                           e is InvalidModelPayloadException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is UserNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e)
                {
                    if (e is DbUpdateException ||
                         e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return Content(HttpStatusCode.InternalServerError, e.Message);
                }
            }
        }
    }
}