using ManagerLayer;
using System;
using System.Web.Http;
using WebApi_PointMap.Models;
using DataAccessLayer.Database;
using WebApi_PointMap.ErrorHandling;

namespace WebApi_PointMap.Controllers
{
    public class PointController : ApiController
    {
        PointManager _pm;
        DatabaseContext _db;

        public PointController()
        {
            _pm = new PointManager();
            _db = new DatabaseContext();
        }

        // GET api/point/get
        [HttpGet]
        [Route("api/point/{guid}")]
        public IHttpActionResult Get(string guid)
        {
            Guid id = new Guid(guid);

            try
            {
                var point = _pm.GetPoint(_db, id);

                _db.SaveChanges();
                return Ok(point);
            }
            catch(Exception e)
            {
                return ResponseMessage(LocalErrorHandler.HandleDatabaseException(e, _db));
            }
        }

        //Post api/point
        [HttpPost]
        [Route("api/point/")]
        public IHttpActionResult Post([FromBody] PointPOST pointPost)
        {
            try
            {
                var point = _pm.CreatePoint(_db, pointPost.Longitude, pointPost.Latitude, pointPost.Description, pointPost.Name);

                _db.SaveChanges();

                return Ok(point);
            }
            catch(Exception e)
            {
                return ResponseMessage(LocalErrorHandler.HandleDatabaseException(e, _db));
            }
        }

        [HttpPut]
        [Route("api/point/{guid}")]
        public IHttpActionResult Put(string guid, [FromBody] PointPOST pointPost)
        {
            Guid id = new Guid(guid);
            pointPost.Id = id;

            try
            {
                var point = _pm.UpdatePoint(_db, id, pointPost.Longitude, pointPost.Latitude,
                                            pointPost.Description, pointPost.Name,
                                            pointPost.CreatedAt);
                _db.SaveChanges();

                return Ok(point);
            }
            catch (Exception e)
            {
                return ResponseMessage(LocalErrorHandler.HandleDatabaseException(e, _db));
            }
        }

        [HttpDelete]
        [Route("api/point/{guid}")]
        public IHttpActionResult Delete(string guid)
        {
            Guid id = new Guid(guid);

            try
            {
                var point = _pm.DeletePoint(_db, id);
                _db.SaveChanges();

                return Ok(point);
            }
            catch (Exception e)
            {
                return ResponseMessage(LocalErrorHandler.HandleDatabaseException(e, _db));
            }
        }
    }
}
