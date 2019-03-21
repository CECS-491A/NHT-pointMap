using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_PointMap.Models;

namespace WebApi_PointMap.Controllers
{
    public class TesterController : ApiController
    {
        // standard route is /api/user
        //  - verbs called on route determine the route pinged

        [HttpGet]
        [Route("api/helloworld")]
        public HttpResponseMessage HelloWorld()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");
            response.Content = new StringContent("Hello World, from NightWatch " + DateTime.Now.ToString(), 
                Encoding.Unicode);
            return response;
        }

        [HttpGet]
        [Route("api/helloworld")]
        public IHttpActionResult HelloWorld(string token)
        {
            return Ok("Hello World, from NightWatch " + DateTime.Now.ToString() + "\nShh... " + token);
        }
    }
}
