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
                        disabled = u.Disabled
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
                            disabled = u.Disabled
                        }).ToList();
                    return Content((HttpStatusCode)200, users);
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
                user.ManagerId = payload.manager;
                try
                {
                    _userManager.UpdateUser(user);
                    _db.SaveChanges();
                    return Content(HttpStatusCode.OK, "User updated");
                }
                catch (Exception)
                {
                    _db.Entry(user).CurrentValues.SetValues(_db.Entry(user).OriginalValues);
                    _db.Entry(user).State = System.Data.Entity.EntityState.Unchanged;
                    return Content(HttpStatusCode.InternalServerError, "User was not updated");
                }
            }
        }

        public class UpdateUserRequestDTO
        {
            [Required]
            public string id { get; set; }
            [Required]
            public string city { get; set; }
            [Required]
            public string state { get; set; }
            [Required]
            public string country { get; set; }
            [Required]
            public Guid manager { get; set; }
            [Required]
            public bool disabled { get; set; }
        }

    }
}
