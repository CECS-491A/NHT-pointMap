using ManagerLayer;
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
using DTO;
using ManagerLayer.Logging;

namespace WebApi_PointMap.Controllers
{
	public class PointController : ApiController
	{
		PointManager _pm;
		DatabaseContext _db;
		LogRequestDTO newLog;
        LoggingManager _lm;

		public PointController()
		{
			_pm = new PointManager();
			_db = new DatabaseContext();
            _lm = new LoggingManager();
		}

		// GET api/point/get
		[HttpGet]
		[Route("api/point/{guid}")]
		public HttpResponseMessage Get(string guid)
		{
			HttpResponseMessage response;

			try
			{ 
				var pointId = ControllerHelpers.ParseAndCheckId(guid);     
				var token = ControllerHelpers.GetToken(Request);
				var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                initalizeLogForController("Session validated at PointController line 45", true);
                newLog = _lm.addSessionToLog(newLog, session);
                _lm.sendLogAsync(newLog);
                Guid id = new Guid(guid);

				var point = _pm.GetPoint(_db, id);
				_db.SaveChanges();

                initalizeLogForController("Request information for MapView in PointController line 51\n " +
                    "route: GET api/point/{guid}", true);
                newLog = _lm.addSessionToLog(newLog, session);
                newLog.page = newLog.mapViewPage;
                _lm.sendLogAsync(newLog);

                response = Request.CreateResponse(HttpStatusCode.OK);
				var jsonContent = new JavaScriptSerializer().Serialize(point);
				response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			}
			catch (Exception e)
			{
				response = PointErrorHandler.HandleException(e, _db);
			}

			return response;
		}

		//Post api/point
		[HttpPost]
		[Route("api/point")]
		public IHttpActionResult Post([FromBody] PointPOST pointPost)
		{
			var token = ControllerHelpers.GetToken(Request);
			var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
            initalizeLogForController("Session validated at PointController line 79", true);
            newLog = _lm.addSessionToLog(newLog, session);
            _lm.sendLogAsync(newLog);
            try
			{
				var point = _pm.CreatePoint(_db, pointPost.Longitude, pointPost.Latitude, pointPost.Description, pointPost.Name);
				_db.SaveChanges();

                initalizeLogForController("Point Creation for MapView in PointController line 85\n " +
                    "route: POST api/point/", true);
                newLog = _lm.addSessionToLog(newLog, session);
                newLog.page = newLog.mapViewPage;
                _lm.sendLogAsync(newLog);

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
			var token = ControllerHelpers.GetToken(Request);
			var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

            initalizeLogForController("Session validated at PointController line 107", true);
            newLog = _lm.addSessionToLog(newLog, session);
            _lm.sendLogAsync(newLog);

            Guid id = new Guid(guid);
			pointPost.Id = id;

			try
			{
				var point = _pm.UpdatePoint(_db, id, pointPost.Longitude, pointPost.Latitude,
											pointPost.Description, pointPost.Name,
											pointPost.CreatedAt);
				_db.SaveChanges();

                initalizeLogForController("Point Edit for MapView in PointController line 118\n " +
                    "route: PUT api/point/{guid}", true);
                newLog = _lm.addSessionToLog(newLog, session);
                newLog.page = newLog.mapViewPage;
                _lm.sendLogAsync(newLog);

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
			var token = ControllerHelpers.GetToken(Request);
			var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

            initalizeLogForController("Session validated at PointController line 142", true);
            newLog = _lm.addSessionToLog(newLog, session);
            _lm.sendLogAsync(newLog);

            Guid id = new Guid(guid);

			try
			{
				_pm.DeletePoint(_db, id);
				_db.SaveChanges();

                initalizeLogForController("Point Deleted for MapView in PointController line 152\n " +
                    "route: DELETE api/point/{guid}", true);
                newLog = _lm.addSessionToLog(newLog, session);
                newLog.page = newLog.mapViewPage;
                _lm.sendLogAsync(newLog);

                return Ok();
			}
			catch (Exception e)
			{
				return ResponseMessage(PointErrorHandler.HandleException(e, _db));
			}
		}

		[HttpGet]
		[Route("api/points")]
		public HttpResponseMessage GetPoints()
		{
			try
			{
				var token = ControllerHelpers.GetToken(Request);
				var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                initalizeLogForController("Session validated at PointController line 176", true);
                newLog = _lm.addSessionToLog(newLog, session);
                _lm.sendLogAsync(newLog);

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

                        initalizeLogForController("Point Deleted for MapView in PointController line 194\n " +
                        "route: GET api/points/", true);
                        newLog = _lm.addSessionToLog(newLog, session);
                        newLog.page = newLog.mapViewPage;
                        _lm.sendLogAsync(newLog);
                    }
					catch(FormatException)
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
			catch(Exception e)
			{
				return PointErrorHandler.HandleException(e, _db);
			}
		}

        private void initalizeLogForController(string details, bool success)
        {
            newLog = new LogRequestDTO();
            newLog.source = "Point Controller";
            newLog.details = details;
            newLog.success = success;
            newLog.page = newLog.sessionPage;
        }
    }
}
