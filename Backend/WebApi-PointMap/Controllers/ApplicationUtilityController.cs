using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi_PointMap.Controllers
{
    public class ApplicationUtilityController : ApiController
    {
        [HttpOptions]
        [Route("api/utility/applicationhealth")]
        public IHttpActionResult ApplicationHealthCheck()
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    _db.SaveChanges();
                    return Content(HttpStatusCode.OK, "PointMap is online.");
                }
                catch (Exception)
                {
                    return Content(HttpStatusCode.InternalServerError, "PointMap is having an internal database error.");
                }
            }
        }

    }
}
