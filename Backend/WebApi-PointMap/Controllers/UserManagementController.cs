using DataAccessLayer.Database;
using DTO.DTO;
using DTO.UserManagementAPI;
using ManagerLayer.UserManagement;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_PointMap.ErrorHandling;
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
                        var users = _userManager.GetUsers();
                        _db.SaveChanges();
                        var responseUsers = Content(HttpStatusCode.OK, users);
                        return responseUsers;
                    }
                    else
                    {
                        _db.SaveChanges(); // save updated user session
                        return Content(HttpStatusCode.Unauthorized, "Non-administrators cannot view all users.");
                    }

                }
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is UserNotFoundException)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is UserIsNotAdministratorException)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception)
                {
                    return InternalServerError();
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
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is InvalidGuidException ||
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
                        var responseDeleted = Content(HttpStatusCode.OK, "User was deleted.");
                        return responseDeleted;
                    }
                    else
                    {
                        _db.SaveChanges(); // save updated user session
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    }
                }
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is UserNotFoundException)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is InvalidModelPayloadException ||
                                            e is UserIsNotAdministratorException)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
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
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is UserNotFoundException)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException || 
                                            e is UserIsNotAdministratorException ||
                                            e is InvalidModelPayloadException)
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
                        // throws exception, invalid username, invalid manager guid
                        var newUser = _userManager.CreateUser(payload);
                        _db.SaveChanges();
                        var responseCreated = Content(HttpStatusCode.Created, "User created.");
                        return responseCreated;
                    }
                    _db.SaveChanges();
                    throw new UserIsNotAdministratorException("Non-administrators can not create users.");
                }
                catch (Exception e) when (e is DbUpdateException ||
                                        e is DbEntityValidationException)
                {
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is UserNotFoundException ||
                                            e is UserAlreadyExistsException ||
                                            e is InvalidEmailException)
                {
                    return ResponseMessage(GeneralErrorHandler.HandleException(e));
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is UserIsNotAdministratorException ||
                                            e is InvalidModelPayloadException)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }
    }
}
