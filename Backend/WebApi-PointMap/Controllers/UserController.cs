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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace WebApi_PointMap.Controllers
{
    public class UserController : ApiController
    {

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
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return DatabaseErrorHandler.HandleException(e, _db);
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is UserAlreadyExistsException ||
                                            e is InvalidEmailException)
                {
                    return GeneralErrorHandler.HandleException(e);
                }
                catch (Exception e) when (e is InvalidTokenSignatureException)
                {
                    return AuthorizationErrorHandler.HandleException(e);
                }
                catch (Exception e) when (e is InvalidModelPayloadException)
                {
                    return HttpErrorHandler.HandleException(e);
                }
                catch (Exception)
                {
                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
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
                    if(user == null)
                    {
                        return Ok();
                    }

                    var _ssoAPIManager = new KFC_SSO_Manager();
                    var requestSuccessful = await _ssoAPIManager.DeleteUserFromSSOviaPointmap(user);
                    if (requestSuccessful)
                    {
                        _userManager.DeleteUserAndSessions(user.Id);
                        _db.SaveChanges();
                        return Ok("User was deleted");
                    }
                    var response = Content(HttpStatusCode.InternalServerError, "User was not deleted.");
                    return response;
                }
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                        e is KFCSSOAPIRequestException ||
                                        e is UserNotFoundException)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception)
                {
                    return InternalServerError();
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
                    // throw exception if user not found
                    var user = _userManager.GetUser(session.UserId);
                    //delete user self and their sessions
                    _userManager.DeleteUserAndSessions(user.Id);
                    _db.SaveChanges();
                    var response = Content(HttpStatusCode.OK, "User was deleted from Pointmap.");
                    return response;
                }
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is UserNotFoundException)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }
        }

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
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is InvalidTokenSignatureException)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                        e is UserNotFoundException)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is InvalidModelPayloadException)
                {
                    return ResponseMessage(HttpErrorHandler.HandleException(e));
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }
        }
    }
}
