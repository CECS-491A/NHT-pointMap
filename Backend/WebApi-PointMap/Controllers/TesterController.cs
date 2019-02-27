using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_PointMap.Models;

namespace WebApi_PointMap.Controllers
{
    [EnableCors(origins: "http://localhost:8080", headers: "*", methods: "*")]
    public class TesterController : ApiController
    {
        // standard route is /api/user
        //  - verbs called on route determine the route pinged

        [HttpGet]
        [Route("api/helloworld")]
        public IHttpActionResult HelloWorld()
        {
            return Ok("Hello World, from NightWatch" + DateTime.Now.ToString());
        }
    }
}
