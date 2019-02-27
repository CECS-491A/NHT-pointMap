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
    public class UserController : ApiController
    {
        // standard route is /api/user
        //  - verbs called on route determine the route pinged

        // GET api/User
        [HttpGet]
        [Route("api/user")]
        public IHttpActionResult Get()
        { 
            // return OK with JSON
            ResponsePOCO tester = new ResponsePOCO{
                Data = new {
                    name = "alfredo",
                    vargas = "asdf"
                },
                Timestamp = DateTime.UtcNow
            };
            return Ok(tester);
        }

        // GET api/User/5
        [HttpGet]
        [Route("api/user/{id}")] //route specific
        public IHttpActionResult Get(int id)
        {
            return Ok(new { id = id });
        }

        // POST api/User
        [HttpPost]
        [Route("api/user")]
        public IHttpActionResult Post([FromBody] UserPOST value) //using a POCO to represent request
        {
            if (value == null)
            {
                return NotFound();
            }
            ResponsePOCO response = new ResponsePOCO{ Data = value, Timestamp = DateTime.UtcNow };
            return Ok(response);
        }

        // PUT api/User/5
        public void Put(int id, [FromBody]string value)
        {
            
        }

        // DELETE api/User/5
        public IHttpActionResult Delete(int id)
        {
            var response = new { id = id };
            return Ok(response);
        }

        // DELETE api/User
        public IHttpActionResult Delete(UserPOST user)
        {
            var response = new { id = user.Username };
            return Ok(response);
        }
    }
}
