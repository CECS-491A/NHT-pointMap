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
using DTO;
using DataAccessLayer.Models;
using Logging.Logging;
namespace WebApi_PointMap.Controllers
{
    public class UserManagementController : ApiController
    {
        Logger logger;
        LogRequestDTO newLog;

        Session session;
        User user;

        public UserManagementController()
        {
            logger = new Logger();
            newLog = new LogRequestDTO();
        }

        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetAllUsers()
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.NoTokenProvidedException
                    var token = ControllerHelpers.GetToken(Request);

                    //throws ExceptionService.SessionNotFoundException
                    session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    newLog = logger.initalizeAnalyticsLog("Session validated at UserManagement Controller line 40\n" +
                        "Route: GET /users/", newLog.sessionSource, session.User, session);
                    logger.sendLogAsync(newLog);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
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

                        newLog = logger.initalizeAnalyticsLog("User list retrieved at UserManagement Controller line 50\n" +
                        "Route: GET /users/", newLog.adminDashSource, session.User, session, newLog.adminDashPage);
                        logger.sendLogAsync(newLog);

                        return Ok(users);
                    }
                    else
                    {
                        _db.SaveChanges(); // save updated user session
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    }

                }
                catch (UserIsNotAdministratorException e)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception e)
                {
                    logger.sendErrorLog(newLog.adminDashSource, e.StackTrace, session.User.Id.ToString(),
                    session.User.Username, newLog.adminDashPage, session);

                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }

        [HttpGet]
        [Route("user")]
        public IHttpActionResult GetUser()
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.NoTokenProvidedException
                    var token = ControllerHelpers.GetToken(Request);

                    //throws ExceptionService.SessionNotFoundException
                    session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    newLog = logger.initalizeAnalyticsLog("Session validated at UserManagement Controller line 96\n" +
                        "Route: GET /user/", newLog.sessionSource, session.User, session);
                    logger.sendLogAsync(newLog);

                    UserManagementManager _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    _db.SaveChanges();

                    newLog = logger.initalizeAnalyticsLog("User retrieved at UserManagement Controller line 103\n" +
                        "Route: GET /user/", newLog.adminDashSource, session.User, session, newLog.adminDashPage);
                    logger.sendLogAsync(newLog);

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
<<<<<<< HEAD
                    logger.sendErrorLog(newLog.adminDashSource, e.StackTrace, session.User.Id.ToString(),
                    session.User.Username, newLog.adminDashPage, session);

=======
                    if (e is SessionNotFoundException || e is NoTokenProvidedException)
                    {
                        return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                    }
>>>>>>> 1a5429c1902b88094286fe212bea0f1db2153d57
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }

        }

        [HttpDelete]
        [Route("user/delete/{userId}")]
        public IHttpActionResult DeleteUser(string userId)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.NoTokenProvidedException
                    var token = ControllerHelpers.GetToken(Request);

                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, userId);

                    //throws ExceptionService.InvalidGuidException
                    var UserId = ControllerHelpers.ParseAndCheckId(userId);

                    //throws ExceptionService.SessionNotFoundException
                    session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    newLog = logger.initalizeAnalyticsLog("Session validated at UserManagement Controller line 144\n" +
                        "Route: DELETE /user/delete/{userId}", newLog.sessionSource, session.User, session);
                    logger.sendLogAsync(newLog);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user.IsAdministrator)
                    {
                        _userManager.DeleteUser(UserId);
                        _db.SaveChanges();

                        newLog = logger.initalizeAnalyticsLog("User deleted at UserManagement Controller line 154\n" +
                        "Route: GET /user/", newLog.adminDashSource, session.User, session, newLog.adminDashPage);
                        logger.sendLogAsync(newLog);

                        return Ok("User was deleted");
                    }
                    else
                    {
                        _db.SaveChanges(); // save updated user session
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    }
                }
<<<<<<< HEAD
                catch (Exception e)
                {
                    logger.sendErrorLog(newLog.adminDashSource, e.StackTrace, session.User.Id.ToString(),
                    session.User.Username, newLog.adminDashPage, session);

                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }

        }

        [HttpPost]
        [Route("sso/user/delete")]
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
                    var _tokenService = new TokenService();
                    if (!_tokenService.isValidSignature(requestPayload.PreSignatureString(), requestPayload.Signature))
                    {
                        throw new InvalidTokenSignatureException("Session is not valid.");
                    }

                    var _userManagementManager = new UserManagementManager(_db);
                    user = _userManagementManager.GetUser(userSSOID);
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
                    _userManagementManager.DeleteUser(userSSOID);
                    _db.SaveChanges();

                    newLog = logger.initalizeAnalyticsLog("User deleted from SSO at UserManagement Controller line 214\n" +
                        "Route: GET /user/", newLog.ssoSource);
                    newLog.ssoUserId = user.Id.ToString();
                    newLog.email = user.Username;
                    logger.sendLogAsync(newLog);

                    return Ok("User was deleted");
=======
                catch (UserIsNotAdministratorException e)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
>>>>>>> 1a5429c1902b88094286fe212bea0f1db2153d57
                }
                catch (Exception e)
                {
                    logger.sendErrorLog(newLog.ssoSource, e.StackTrace, user.Id.ToString(), user.Username);

                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }

        }

        [HttpPut]
        [Route("user/update")]
        public IHttpActionResult UpdateUser([FromBody] UpdateUserRequestDTO payload)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.NoTokenProvidedException
                    var token = ControllerHelpers.GetToken(Request);

                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, payload);

                    //throws ExceptionService.InvalidGuidException
                    var UserId = ControllerHelpers.ParseAndCheckId(payload.Id);

                    //throws ExceptionService.SessionNotFoundException
                    session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    newLog = logger.initalizeAnalyticsLog("Session validated at UserManagement Controller line 250\n" +
                        "Route: PUT /user/update", newLog.sessionSource, session.User, session);
                    logger.sendLogAsync(newLog);

                    var _userManager = new UserManagementManager(_db);
                    var manager = _userManager.GetUser(session.UserId);
                    if (manager.IsAdministrator)
                    {
                        var user = _userManager.GetUser(UserId);
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

                        _userManager.UpdateUser(user);
                        _db.SaveChanges();

                        newLog = logger.initalizeAnalyticsLog("User updated at UserManagement Controller line 278\n" +
                        "Route: PUT /user/update", newLog.adminDashSource, session.User, session, newLog.adminDashPage);
                        logger.sendLogAsync(newLog);

                        return Content(HttpStatusCode.OK, "User updated");
                    }
                    else
                    {
                        _db.SaveChanges(); // save updated user session
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    };
                }
                catch (UserNotFoundException e)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e)
                {
                    logger.sendErrorLog(newLog.adminDashSource, e.StackTrace, session.User.Id.ToString(),
                    session.User.Username, newLog.adminDashPage, session);

                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }
    }
}
