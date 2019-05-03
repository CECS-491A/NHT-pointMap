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
        // Check the health of the application
        //  - return status code based on internal applicaiton errors
        [HttpGet]
        [Route("api/utility/applicationhealth")]
        public IHttpActionResult ApplicationHealthCheck()
        {
            using (var _db = new DatabaseContext())
            {
                try
                {
                    var existingConnection = _db.Database.Exists();
                    if (!existingConnection)
                    {
                        return Content(HttpStatusCode.InternalServerError, "PointMap is encountering problems. (database connection error)");
                    }
                    _db.SaveChanges();
                    return Content(HttpStatusCode.OK, "PointMap is online.");
                }
                catch (Exception) // catch error when trying to call db, return status of internal problems
                {
                    return Content(HttpStatusCode.InternalServerError, "PointMap is encountering problems. (internal database errors)");
                }
            }
        }

    }
}
