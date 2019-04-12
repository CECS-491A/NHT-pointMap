﻿using ManagerLayer;
using System;
using System.Web.Http;
using WebApi_PointMap.Models;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using ManagerLayer.AccessControl;
using System.Web.Script.Serialization;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using WebApi_PointMap.ErrorHandling;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.Controllers
{
    public class PointController : ApiController
    {
        PointManager _pm;
	    DatabaseContext _db;
        AuthorizationManager _am;

        public PointController()
        {
            _pm = new PointManager();
            _am = new AuthorizationManager();
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
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }

        //Post api/point
        [HttpPost]
        [Route("api/point")]
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
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
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
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }

        [HttpDelete]
        [Route("api/point/{guid}")]
        public IHttpActionResult Delete(string guid)
        {
            Guid id = new Guid(guid);

            try
            {
                _pm.DeletePoint(_db, id);
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }

        [HttpGet]
        [Route("api/points")]
        public IHttpActionResult GetPoints()
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request, "token");

                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                var headers = Request.Headers;

                if(headers.Contains("minLng") && headers.Contains("maxLng") && 
                    headers.Contains("minLat") && headers.Contains("maxLat"))
                {
                    object pointList;
                    try
                    {
                        float minLng = float.Parse(headers.GetValues("minLng").First());
                        float minLat = float.Parse(headers.GetValues("minLat").First());
                        float maxLng = float.Parse(headers.GetValues("maxLng").First());
                        float maxLat = float.Parse(headers.GetValues("maxLat").First());
                        pointList = _pm.GetAllPoints(_db, minLat, minLng, maxLat, maxLng);
                    }
                    catch(FormatException)
                    {
                        throw new InvalidHeaderException("Invalid field formatting.");
                    }
                            
                    if (pointList != null)
                    {
                        var jsonContent = new JavaScriptSerializer().Serialize(pointList);
                        return Ok(new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                    }
                }
                throw new InvalidHeaderException("Invalid field formatting.");
            }
            catch(Exception e)
            {
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }
    }
}
