using DataAccessLayer.Database;
using DTO.DTO;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApi_PointMap.ErrorHandling;
using WebApi_PointMap.Models;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.Controllers
{
    public class UserManagementController : ApiController
    {
        DatabaseContext _db;
        UserManagementManager _userManager;

        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                //throws ExceptionService.NoTokenProvidedException
                var token = ControllerHelpers.GetToken(Request, "Token");
                _db = new DatabaseContext();

                //throws ExceptionService.SessionNotFoundException
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                _userManager = new UserManagementManager();
                var user = _userManager.GetUser(_db, session.UserId);
                if (user.IsAdministrator)
                {
                    var users = _db.Users
                    .Select(u => new
                    {
                        id = u.Id,
                        username = u.Username,
                        manager = u.ManagerId,
                        city = u.City,
                        state = u.State,
                        country = u.Country,
                        disabled = u.Disabled,
                        isAdmin = u.IsAdministrator
                    }).ToList();
                    _db.SaveChanges();
                    return Ok(users);
                }
                else
                {
                    _db.SaveChanges(); // save updated user session
                    throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                }
            }
            catch (Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }

        [HttpGet]
        [Route("users/{managerId}")]
        public IHttpActionResult GetUsersUnderManager(string managerId)
        {
            try
            {
                //throws ExceptionService.InvalidModelPayloadException
                ControllerHelpers.ValidateModelAndPayload(ModelState, managerId);

                //throws ExceptionService.InvalidGuidException
                var ManagerId = ControllerHelpers.ParseAndCheckId(managerId);

                _db = new DatabaseContext();

                var users = _db.Users
                    .Where(u => u.Manager.Id == ManagerId)
                    .Select(u => new {
                        id = u.Id,
                        username = u.Username,
                        manager = u.ManagerId,
                        city = u.City,
                        state = u.State,
                        country = u.Country,
                        disabled = u.Disabled,
                        isAdmin = u.IsAdministrator
                    }).ToList();
                return Ok(users);
            }
            catch (Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }

        [HttpGet]
        [Route("user")]
        public IHttpActionResult GetUser()
        {
            try
            {
                //throws ExceptionService.NoTokenProvidedException
                var token = ControllerHelpers.GetToken(Request, "Token");

                _db = new DatabaseContext();

                //throws ExceptionService.SessionNotFoundException
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                UserManagementManager _userManager = new UserManagementManager();
                var user = _userManager.GetUser(_db, session.UserId);
                _db.SaveChanges();
                //could be replaced with return Ok(user)?
                return Ok(new
                {
                    id = user.Id,
                    username = user.Username,
                    disabled = user.Disabled,
                    isAdmin = user.IsAdministrator
                });
            }
            catch (Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }

        [HttpDelete]
        [Route("user/delete/{userId}")]
        public IHttpActionResult DeleteUser(string userId)
        {
            try
            {
                //throws ExceptionService.NoTokenProvidedException
                var token = ControllerHelpers.GetToken(Request, "Token");

                //throws ExceptionService.InvalidModelPayloadException
                ControllerHelpers.ValidateModelAndPayload(ModelState, userId);

                //throws ExceptionService.InvalidGuidException
                var UserId = ControllerHelpers.ParseAndCheckId(userId);
                _db = new DatabaseContext();

                //throws ExceptionService.SessionNotFoundException
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                UserManagementManager _userManager = new UserManagementManager();
                var user = _userManager.GetUser(_db, session.UserId);
                if (user.IsAdministrator)
                {
                    _userManager.DeleteUser(_db, UserId);
                    _db.SaveChanges();
                    return Ok("User was deleted");
                }
                else
                {
                    _db.SaveChanges(); // save updated user session
                    throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                }
            }
            catch (Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }

        [HttpPost]
        [Route("sso/user/delete")]
        public IHttpActionResult DeleteUser([FromBody, Required] LoginDTO requestPayload)
        {  
            try
            {
                //throws ExceptionService.InvalidModelPayloadException
                ControllerHelpers.ValidateModelAndPayload(ModelState, requestPayload);

                //throws ExceptionService.InvalidGuidException
                var userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                // check valid signature
                TokenService _tokenService = new TokenService();
                if (!_tokenService.isValidSignature(requestPayload.PreSignatureString(), requestPayload.Signature))
                {
                    throw new InvalidTokenSignatureException("Session is not valid.");
                }

                _db = new DatabaseContext();

                var _userManagementManager = new UserManagementManager();
                var user = _userManagementManager.GetUser(_db, userSSOID);
                if (user == null)
                {
                    return Ok("User was never registered.");
                }

                var _sessionService = new SessionService();
                var sessions = _sessionService.GetSessions(_db, userSSOID);
                if (sessions != null)
                {
                    foreach (var sess in sessions)
                    {
                        _sessionService.DeleteSession(_db, sess.Token);
                    }
                }

                UserManagementManager _userManager = new UserManagementManager();
                _userManager.DeleteUser(_db, userSSOID);
                _db.SaveChanges();
                return Ok("User was deleted");
            }
            catch (Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }

        [HttpPut]
        [Route("user/update")]
        public IHttpActionResult UpdateUser([FromBody] UpdateUserRequestDTO payload)
        {
            try
            {
                //throws ExceptionService.NoTokenProvidedException
                var token = ControllerHelpers.GetToken(Request, "Token");
                
                //throws ExceptionService.InvalidModelPayloadException
                ControllerHelpers.ValidateModelAndPayload(ModelState, payload);

                //throws ExceptionService.InvalidGuidException
                var UserId = ControllerHelpers.ParseAndCheckId(payload.Id);
                _db = new DatabaseContext();

                //throws ExceptionService.SessionNotFoundException
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                var _userManager = new UserManagementManager();
                var manager = _userManager.GetUser(_db, session.UserId);
                if (manager.IsAdministrator)
                {
                    var user = _userManager.GetUser(_db, UserId);
                    if (user == null)
                    {
                        throw new UserNotFoundException("User does not exist.");
                    }
                    user.City = payload.City;
                    user.State = payload.State;
                    user.Country = payload.Country;
                    user.Disabled = payload.Disabled;
                    user.IsAdministrator = payload.IsAdmin;
                    user.ManagerId = null;
                    if (payload.Manager != null)
                    {
                        //no need to check for parse error here (managerId is already in the database)
                        var managerId = Guid.Parse(payload.Manager);
                        user.ManagerId = managerId;
                    }

                    _userManager.UpdateUser(_db, user);
                    _db.SaveChanges();
                    return Content(HttpStatusCode.OK, "User updated");
                }
                else
                {
                    _db.SaveChanges(); // save updated user session
                    throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                };
            }
            catch (Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }
    }
}
