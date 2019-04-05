using DataAccessLayer.Database;
using ManagerLayer.UserManagement;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                    return Ok(users);
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }
        }

        [HttpGet]
        [Route("users/{managerId}")]
        public IHttpActionResult GetUsersUnderManager(string managerId)
        {
            if (managerId == null)
            {
                return Content((HttpStatusCode)412, "Invalid payload.");
            }
            Guid ManagerId;
            try
            {
                ManagerId = Guid.Parse(managerId);
            }
            catch (Exception)
            {
                return Content((HttpStatusCode)400, "Invalid SSO ID");
            }
            using (var _db = new DatabaseContext())
            {
                try
                {
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
                catch (Exception)
                {
                    return InternalServerError();
                }
            }
        }

        [HttpGet]
        [Route("user/{token}")]
        public IHttpActionResult GetUser(string token)
        {
            if (token == null || token == "")
            {
                return BadRequest("Invalid Session.");
            }
            using (var _db = new DatabaseContext())
            {
                SessionService _sessionService = new SessionService();
                try
                {
                    var session = _sessionService.ValidateSession(_db, token);
                    if (session == null)
                    {
                        return Content(HttpStatusCode.NotFound, "Session is no longer available.");
                    }
                    UserManagementManager _userManager = new UserManagementManager(_db);
                    var user = _userManager.GetUser(session.UserId);
                    return Ok(new
                    {
                        id = user.Id,
                        username = user.Username,
                        disabled = user.Disabled,
                        isAdmin = user.IsAdministrator
                    });
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.InternalServerError, ex);
                }

            }
        }

        [HttpDelete]
        [Route("user/delete/{userId}")]
        public IHttpActionResult DeleteUser(string userId)
        {
            if (userId == null)
            {
                return Content((HttpStatusCode)412, "Invalid payload.");
            }
            Guid UserId;
            try
            {
                // check if valid SSO ID format
                UserId = Guid.Parse(userId);
            }
            catch (Exception)
            {
                return Content((HttpStatusCode)400, "Invalid User ID");
            }
            using(var _db = new DatabaseContext())
            {
                UserManagementManager _userManager = new UserManagementManager(_db);
                try
                {
                    _userManager.DeleteUser(UserId);
                    _db.SaveChanges();
                    return Ok("User was deleted");
                }
                catch (Exception ex)
                {
                    return Content((HttpStatusCode)500, ex.Message);
                }
            }
        }

        [HttpPut]
        [Route("user/update")]
        public IHttpActionResult UpdateUser([FromBody] UpdateUserRequestDTO payload)
        {
            if (!ModelState.IsValid || payload == null)
            {
                return Content((HttpStatusCode)412, ModelState);
            }
            Guid UserId;
            try
            {
                // check if valid SSO ID format
                UserId = Guid.Parse(payload.id);
            }
            catch (Exception)
            {
                return Content((HttpStatusCode)400, "Invalid User ID");
            }
            using (var _db = new DatabaseContext())
            {
                UserManagementManager _userManager = new UserManagementManager(_db);
                var user = _userManager.GetUser(UserId);
                if (user == null)
                {
                    return Content(HttpStatusCode.NotFound, "User does not exist.");
                }
                user.City = payload.city;
                user.State = payload.state;
                user.Country = payload.country;
                user.Disabled = payload.disabled;
                user.IsAdministrator = payload.isAdmin;
                try
                {
                    var ManagerId = Guid.Parse(payload.manager);
                    user.ManagerId = ManagerId;
                }
                catch (Exception)
                {
                    user.ManagerId = null;
                }
                try
                {
                    _userManager.UpdateUser(user);
                    _db.SaveChanges();
                    return Content(HttpStatusCode.OK, "User updated");
                }
                catch (Exception ex)
                {
                    _db.Entry(user).CurrentValues.SetValues(_db.Entry(user).OriginalValues);
                    _db.Entry(user).State = System.Data.Entity.EntityState.Unchanged;
                    return Content(HttpStatusCode.InternalServerError, ex);
                }
            }
        }

        public class UpdateUserRequestDTO
        {
            [Required]
            public string id { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string manager { get; set; }
            [Required]
            public bool isAdmin { get; set; }
            [Required]
            public bool disabled { get; set; }
        }

    }
}
