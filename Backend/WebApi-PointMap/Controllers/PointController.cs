using System;
using System.Web.Http;
using ManagerLayer;
using WebApi_PointMap.Models;

namespace WebApi_PointMap.Controllers
{
    public class PointController : ApiController
    {
        PointManager _pm;

        public PointController()
        {
            _pm = new PointManager();
        }
        // GET api/point/get
        [HttpGet]
        [Route("api/point/{guid}")]
        public IHttpActionResult Get(string guid)
        {
            Guid id = new Guid(guid);
            try
            {
                var point = _pm.GetPoint(id);
                return Ok(point);
            }
            catch(Exception e)
            {
                return Ok(e.StackTrace);
            }
        }

        //Post api/point
        [HttpPost]
        [Route("api/point/")]
        public IHttpActionResult Post([FromBody] PointPOST pointPost)
        {
            try
            {
                var point = _pm.CreatePoint(pointPost.Longitude, pointPost.Latitude, pointPost.Description, pointPost.Name);
                return Ok(point);
            }
            catch(ArgumentOutOfRangeException e)
            {
                return BadRequest(e.ParamName);
            }
            catch(Exception e)
            {
                return Ok(e.StackTrace);
            }
        }

        [HttpPut]
        [Route("api/point/{guid}")]
        public IHttpActionResult Put(string guid, [FromBody] PointPOST pointPost)
        {
            Guid id = new Guid(guid);
            try
            {
                pointPost.Id = id;
                var point = _pm.UpdatePoint(id, pointPost.Longitude, pointPost.Latitude,
                                                pointPost.Description, pointPost.Name, 
                                                pointPost.CreatedAt);

                return Ok(point);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.ParamName);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpDelete]
        [Route("api/point/{guid}")]
        public IHttpActionResult Delete(string guid)
        {
            Guid id = new Guid(guid);
            try
            {
                _pm.DeletePoint(id);
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.StackTrace);
            }
        }
    }
}
