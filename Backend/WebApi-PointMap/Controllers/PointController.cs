using ManagerLayer;
using System;
using System.Web.Http;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using DataAccessLayer.Database;
using static ServiceLayer.Services.ExceptionService;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using DTO.PointAPI;

namespace WebApi_PointMap.Controllers
{
    public class PointController : ApiController
    {
        private PointManager _pm;
	    private DatabaseContext _db;

        // retrieves a point
        [HttpGet]
        [Route("api/point/{guid}")]
        public IHttpActionResult Get(string guid)
        {
            using (_db = new DatabaseContext())
            {
                try
                {
                    var pointId = ControllerHelpers.ParseAndCheckId(guid);
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    _pm = new PointManager(_db);
                    var point = _pm.GetPoint(pointId);

                    return Ok(point);
                }
                catch (Exception e) when (e is InvalidPointException ||
                                            e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is PointNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e)
                {
                    //Revert database changes if necessary
                    if(e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return InternalServerError();
                }
            }
        }

        // creates a point
        [HttpPost]
        [Route("api/point")]
        public IHttpActionResult Post([FromBody] PointRequestDTO pointPost)
        {
            using (_db = new DatabaseContext())
            {
                try
                {
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);
                    ControllerHelpers.ValidateModelAndPayload(ModelState, pointPost);

                    _pm = new PointManager(_db);
                    var point = _pm.CreatePoint(pointPost.Longitude, pointPost.Latitude, pointPost.Description, pointPost.Name);

                    _db.SaveChanges();

                    return Content(HttpStatusCode.Created, point);
                }
                catch (Exception e) when (e is InvalidPointException ||
                                            e is InvalidGuidException ||
                                            e is InvalidModelPayloadException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is PointNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e)
                {
                    //Revert database changes if necessary
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return InternalServerError();
                }
            }
        }

        // updates a point
        [HttpPut]
        [Route("api/point")]
        public IHttpActionResult Put([FromBody] PointRequestDTO pointPost)
        {
            using (_db = new DatabaseContext())
            {
                try
                {
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request); ;
                    ControllerHelpers.ValidateModelAndPayload(ModelState, pointPost);

                    var pointId = ControllerHelpers.ParseAndCheckId(pointPost.Id.ToString());

                    _pm = new PointManager(_db);
                    var point = _pm.UpdatePoint(pointId, pointPost.Longitude, pointPost.Latitude,
                                                pointPost.Description, pointPost.Name,
                                                pointPost.CreatedAt);

                    _db.SaveChanges();

                    return Ok(point);
                }
                catch (Exception e) when (e is InvalidPointException ||
                                            e is InvalidGuidException ||
                                            e is InvalidModelPayloadException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is PointNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e)
                {
                    //Revert database changes if necessary
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return InternalServerError();
                }
            }
        }

        //Deletes a point
        [HttpDelete]
        [Route("api/point/{guid}")]
        public IHttpActionResult Delete(string guid)
        {
            using (_db = new DatabaseContext())
            {
                try
                {
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    var pointId = ControllerHelpers.ParseAndCheckId(guid);

                    _pm = new PointManager(_db);
                    _pm.DeletePoint(pointId);

                    _db.SaveChanges();

                    return Ok();
                }
                catch (Exception e) when (e is InvalidPointException ||
                                            e is InvalidGuidException)
                {
                    return Content(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is PointNotFoundException)
                {
                    return Content(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return Content(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e)
                {
                    //Revert database changes if necessary
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return InternalServerError();
                }
            }
        }

        [HttpGet]
        [Route("api/points")]
        public HttpResponseMessage GetPoints()
        {
            using (_db = new DatabaseContext())
            {
                try
                {
                    var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                    var headers = Request.Headers;

                    if (headers.Contains("minLng") && headers.Contains("maxLng") &&
                        headers.Contains("minLat") && headers.Contains("maxLat"))
                    {
                        object pointList;
                        try
                        {
                            float minLng = float.Parse(headers.GetValues("minLng").First());
                            float minLat = float.Parse(headers.GetValues("minLat").First());
                            float maxLng = float.Parse(headers.GetValues("maxLng").First());
                            float maxLat = float.Parse(headers.GetValues("maxLat").First());
                            _pm = new PointManager(_db);
                            pointList = _pm.GetAllPoints(minLat, minLng, maxLat, maxLng);
                        }
                        catch (FormatException)
                        {
                            throw new InvalidHeaderException("Invalid field formatting.");
                        }

                        if (pointList != null)
                        {
                            var jsonContent = new JavaScriptSerializer().Serialize(pointList);
                            var response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                            return response;
                        }
                    }
                    throw new InvalidHeaderException("Invalid field formatting.");
                }
                catch (Exception e) when (e is InvalidPointException ||
                                            e is InvalidGuidException ||
                                            e is InvalidModelPayloadException ||
                                            e is InvalidHeaderException)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
                }
                catch (Exception e) when (e is PointNotFoundException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, e.Message);
                }
                catch (Exception e) when (e is NoTokenProvidedException ||
                                            e is SessionNotFoundException)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, e.Message);
                }
                catch (Exception e)
                {
                    //Revert database changes if necessary
                    if (e is DbUpdateException ||
                        e is DbEntityValidationException)
                    {
                        _db.RevertDatabaseChanges(_db);
                    }
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}
