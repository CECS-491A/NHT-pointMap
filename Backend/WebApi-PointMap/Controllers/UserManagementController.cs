using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DTO.DTO;
using ManagerLayer.AccessControl;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_PointMap.ErrorHandling;
using WebApi_PointMap.Models;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.Controllers
{
    public class UserManagementController : ApiController
    {
        DatabaseContext _db;
        SessionService _sessionService;
        UserManagementManager _userManager;

        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                var token = ControllerHelpers.GetAndCheckToken(Request, "Token");
                _db = new DatabaseContext();

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
                _db.SaveChanges();
                return Unauthorized();
            }
            catch (Exception e)
            {
                return ResponseMessage(UserErrorHandler.HandleException(e, _db));
            }
        }

        [HttpGet]
        [Route("users/{managerId}")]
        public IHttpActionResult GetUsersUnderManager(string managerId)
        {
            try
            {
                if (managerId == null)
                {
                    throw new FormatException("Invalid payload.");
                }

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
                return Content((HttpStatusCode)200, users);
            }
            catch (Exception e)
            {
                return ResponseMessage(UserErrorHandler.HandleException(e, _db));
            }
        }

        [HttpGet]
        [Route("user")]
        public IHttpActionResult GetUser()
        {
            try
            {
                var token = ControllerHelpers.GetAndCheckToken(Request, "Token");

                _db = new DatabaseContext();

                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                UserManagementManager _userManager = new UserManagementManager();
                var user = _userManager.GetUser(_db, session.UserId);
                _db.SaveChanges();
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
                return ResponseMessage(UserErrorHandler.HandleException(e, _db));
            }
        }

        [HttpDelete]
        [Route("user/delete/{userId}")]
        public IHttpActionResult DeleteUser(string userId)
        {
            try
            {
                var token = ControllerHelpers.GetAndCheckToken(Request, "Token");
                _db = new DatabaseContext();

                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                if (userId == null)
                {
                    throw new FormatException("Invalid payload.");
                }

                var UserId = ControllerHelpers.ParseAndCheckId(userId);

                UserManagementManager _userManager = new UserManagementManager();
                var user = _userManager.GetUser(_db, session.UserId);
                if (user.IsAdministrator)
                {
                    _userManager.DeleteUser(_db, UserId);
                    _db.SaveChanges();
                    return Ok("User was deleted");
                }
                _db.SaveChanges();
                return Unauthorized();
            }
            catch (Exception e)
            {
                return ResponseMessage(UserErrorHandler.HandleException(e, _db));
            }
        }

        [HttpPost]
        [Route("sso/user/delete")]
        public IHttpActionResult DeleteUser([FromBody, Required] LoginDTO requestPayload)
        {
            if (!ModelState.IsValid || requestPayload == null)
            {
                return Content((HttpStatusCode)412, ModelState);
            }
            try
            {
                var userSSOID = ControllerHelpers.ParseAndCheckId(requestPayload.SSOUserId);

                // check valid signature
                TokenService _tokenService = new TokenService();
                if (!_tokenService.isValidSignature(requestPayload.PreSignatureString(), requestPayload.Signature))
                {
                    throw new InvalidTokenSignatureException("Session is not valid.");
                }

                _db = new DatabaseContext();

                List<Session> sessions = null;
                User user = null;

                var _userManagementManager = new UserManagementManager();
                user = _userManagementManager.GetUser(_db, userSSOID);
                if (user == null)
                {
                    return Ok("User was never registered.");
                }
                var _sessionService = new SessionService();
                sessions = _sessionService.GetSessions(_db, userSSOID);
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
                return ResponseMessage(UserErrorHandler.HandleException(e, _db));
            }
        }

        [HttpPut]
        [Route("user/update")]
        public IHttpActionResult UpdateUser([FromBody] UpdateUserRequestDTO payload)
        {
            try
            {
                var token = ControllerHelpers.GetAndCheckToken(Request, "Token");
                if (!ModelState.IsValid || payload == null)
                {
                    return Content((HttpStatusCode)412, ModelState);
                }
                _db = new DatabaseContext();

                var UserId = ControllerHelpers.ParseAndCheckId(payload.Id);

                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                UserManagementManager _userManager = new UserManagementManager();
                var byUser = _userManager.GetUser(_db, session.UserId);
                if (byUser.IsAdministrator)
                {
                    var user = _userManager.GetUser(_db, UserId);
                    if (user == null)
                    {
                        throw new NullReferenceException("User does not exist.");
                    }
                    user.City = payload.City;
                    user.State = payload.State;
                    user.Country = payload.Country;
                    user.Disabled = payload.Disabled;
                    user.IsAdministrator = payload.IsAdmin;
                    user.ManagerId = null;
                    if (payload.Manager != null)
                    {
                        var ManagerId = Guid.Parse(payload.Manager);
                        user.ManagerId = ManagerId;
                    }

                    _userManager.UpdateUser(_db, user);
                    _db.SaveChanges();
                    return Content(HttpStatusCode.OK, "User updated");
                }
                _db.SaveChanges();
                return Unauthorized();
            }
            catch (Exception e)
            {
                return ResponseMessage(UserErrorHandler.HandleException(e, _db));
            }
        }
    }
}
