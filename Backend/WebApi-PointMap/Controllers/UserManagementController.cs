using DataAccessLayer.Database;
using DTO.DTO;
using DTO.UserManagementAPI;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
using System;
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
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user.IsAdministrator)
                    {
                        var users = _db.Users
                        .Select(u => new GetAllUsersResponseDataItem
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
                        var responseUsers = Content(HttpStatusCode.OK, users);
                        return responseUsers;
                    }
                    else
                    {
                        _db.SaveChanges(); // save updated user session
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    }

                }
                catch (Exception e)
                {
                    if (e is NoTokenProvidedException || e is InvalidModelPayloadException || e is UserIsNotAdministratorException || e is SessionNotFoundException)
                    {
                        var responseUnauthorized = ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                        return responseUnauthorized;
                    }
                    if (e is UserNotFoundException)
                    {
                        _db.RevertDatabaseChanges(_db);
                        return ResponseMessage(GeneralErrorHandler.HandleException(e));
                    }
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
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    UserManagementManager _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    _db.SaveChanges();
                    var responseData = new GetUserResponseData
                    {
                        id = user.Id,
                        username = user.Username,
                        disabled = user.Disabled,
                        isAdmin = user.IsAdministrator
                    };
                    var responseUser = Content(HttpStatusCode.OK, responseData);
                    return responseUser;
                }
                catch (Exception e)
                {
                    if (e is UserNotFoundException)
                    {
                        _db.RevertDatabaseChanges(_db);
                        return ResponseMessage(GeneralErrorHandler.HandleException(e));
                    }
                    if (e is SessionNotFoundException || e is NoTokenProvidedException)
                    {
                        return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                    }
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
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user.IsAdministrator)
                    {
                        _userManager.DeleteUser(UserId);
                        _db.SaveChanges();
                        var responseDeleted = Content(HttpStatusCode.OK, "User was delted.");
                        return responseDeleted;
                    }
                    else
                    {
                        _db.SaveChanges(); // save updated user session
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    }
                }
                catch (Exception e)
                {
                    if (e is NoTokenProvidedException || e is InvalidModelPayloadException || e is UserIsNotAdministratorException || e is SessionNotFoundException)
                    {
                        var responseUnauthorized = ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                        return responseUnauthorized;
                    }
                    if (e is UserNotFoundException)
                    {
                        _db.RevertDatabaseChanges(_db);
                        return ResponseMessage(GeneralErrorHandler.HandleException(e));
                    }
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
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user.IsAdministrator)
                    {
                        var userToUpdate = _userManager.GetUser(UserId);
                        _userManager.ToUpdateUser(userToUpdate, payload);

                        _userManager.UpdateUser(userToUpdate);
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
                    if (e is NoTokenProvidedException || 
                        e is InvalidModelPayloadException || 
                        e is UserIsNotAdministratorException || 
                        e is SessionNotFoundException)
                    {
                        var responseUnauthorized = ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                        return responseUnauthorized;
                    }
                    if (e is UserNotFoundException ||
                        e is InvalidGuidException)
                    {
                        _db.RevertDatabaseChanges(_db);
                        return ResponseMessage(GeneralErrorHandler.HandleException(e));
                    }
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }

        [HttpPost]
        [Route("user/create")]
        public IHttpActionResult CreateNewUser([FromBody, Required] CreateUserRequestDTO payload)
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    //throws ExceptionService.NoTokenProvidedException
                    var token = ControllerHelpers.GetToken(Request);

                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, payload);

                    //throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                    var _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    if (user.IsAdministrator)
                    {
                        var newUser = _userManager.CreateUser(payload);
                        _db.SaveChanges();
                        var responseCreated = Content(HttpStatusCode.Created, "User created.");
                        return responseCreated;
                    }
                    _db.SaveChanges();
                    throw new UserIsNotAdministratorException("Non-administrators can not create users.");
                }
                catch (Exception e)
                {
                    if (e is UserIsNotAdministratorException)
                    {
                        var responseUnauthorized = ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                        return responseUnauthorized;
                    }
                    if (e is InvalidEmailException || e is InvalidGuidException || e is UserNotFoundException)
                    {
                        _db.RevertDatabaseChanges(_db);
                        var responseInvalidEmail = ResponseMessage(GeneralErrorHandler.HandleException(e));
                        return responseInvalidEmail;
                    }
                    var responseInternalError = ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                    return responseInternalError;
                }
            }
        }
    }
}
