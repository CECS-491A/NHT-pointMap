using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApi_PointMap.Controllers
{
    public class TesterController : ApiController
    {
        // standard route is /api/user
        //  - verbs called on route determine the route pinged

        [HttpGet]
        [Route("api/helloworld")]
        public IHttpActionResult HelloWorld()
        {
            return Ok("Hello World, from NightWatch " + DateTime.Now.ToString());
        }

        [HttpGet]
        [Route("api/helloworld")]
        public IHttpActionResult HelloWorld(string token)
        {
            return Ok("Hello World, from NightWatch " + DateTime.Now.ToString() + "\nShh... " + token);
        }
    }
}
