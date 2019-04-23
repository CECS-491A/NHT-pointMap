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
using static DTO.DTO.SSOServicesDTOs;
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
                catch (UserIsNotAdministratorException e)
                {
                    return ResponseMessage(AuthorizationErrorHandler.HandleException(e));
                }
                catch (Exception e)
                {
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
                        return Ok("User was deleted");
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
                    return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
                }
            }
        }
    }
}
