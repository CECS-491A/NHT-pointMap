using DataAccessLayer.Database;
using DTO.UserManagementAPI;
using ManagerLayer.UserManagement;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Net;
using System.Web.Http;
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
                    //throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

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
                        return Content(HttpStatusCode.Unauthorized, "Non-administrators cannot view all users.");
                    }

                }
                catch (Exception e) when (e is UserNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is UserIsNotAdministratorException)
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
                    //throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

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
                catch (Exception e) when (e is UserNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is UserIsNotAdministratorException)
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
                    //throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, userId);

                    //throws ExceptionService.InvalidGuidException
                    var UserId = ControllerHelpers.ParseAndCheckId(userId);

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
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    }
                }
                catch (Exception e) when (e is UserNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is UserIsNotAdministratorException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e) when (e is InvalidModelPayloadException)
                {
                    return Content(HttpStatusCode.PreconditionFailed, e.Message);
                }
                catch (Exception e)
                {
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return InternalServerError();
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
                    //throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, payload);

                    //throws ExceptionService.InvalidGuidException
                    var UserId = ControllerHelpers.ParseAndCheckId(payload.Id);


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
                        throw new UserIsNotAdministratorException("Non-administrators cannot delete users.");
                    }
                }
                catch (Exception e) when (e is UserNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is UserIsNotAdministratorException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e) when (e is InvalidModelPayloadException)
                {
                    return Content(HttpStatusCode.PreconditionFailed, e.Message);
                }
                catch (Exception e)
                {
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
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
                    //throws ExceptionService.SessionNotFoundException
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    //throws ExceptionService.InvalidModelPayloadException
                    ControllerHelpers.ValidateModelAndPayload(ModelState, payload);

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
                    throw new UserIsNotAdministratorException("Non-administrators can not create users.");
                }
                catch (Exception e) when (e is UserNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is InvalidGuidException ||
                                            e is InvalidEmailException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException ||
                                            e is UserIsNotAdministratorException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e) when (e is InvalidModelPayloadException)
                {
                    return Content(HttpStatusCode.PreconditionFailed, e.Message);
                }
                catch (Exception e) when (e is UserAlreadyExistsException)
                {
                    return Content(HttpStatusCode.Conflict, e.Message);
                }
                catch (Exception e)
                {
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return InternalServerError();
                }
            }
        }
    }
}
