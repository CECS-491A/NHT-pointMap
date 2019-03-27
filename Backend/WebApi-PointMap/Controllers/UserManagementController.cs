using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
                    .Select(p => new
                    {
                        id = p.Id,
                        username = p.Username,
                        manager = p.ManagerId
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
                            manager = u.ManagerId
                        }).ToList();
                    return Content((HttpStatusCode)200, users);
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }
        }

    }
}
