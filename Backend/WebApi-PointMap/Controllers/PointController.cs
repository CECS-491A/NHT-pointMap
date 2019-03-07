using System;
using System.Web.Http;
using ManagerLayer;

namespace WebApi_PointMap.Controllers
{
    public class PointController : ApiController
    {
        ManagerLayer.PointManager _pm;

        public PointController()
        {
            _pm = new PointManager();
        }
        // GET api/point/get
        [HttpGet]
        [Route("point/get/{Id}")]
        public IHttpActionResult Get(Guid Id)
        {

            return Ok("point");
        }
    }
}
