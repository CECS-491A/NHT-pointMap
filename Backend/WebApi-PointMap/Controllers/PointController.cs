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

namespace WebApi_PointMap.Controllers
{
    public class PointController : ApiController
    {
        PointManager _pm;
        AuthorizationManager _am;

        public PointController()
        {
            _pm = new PointManager();
            _am = new AuthorizationManager();
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

        [HttpGet]
        [Route("api/points/")]
        public HttpResponseMessage getPoints()
        {
            HttpResponseMessage response;
            var re = Request;
            var headers = re.Headers;
            if (headers.Contains("token"))
            {
                string token = headers.GetValues("token").First();
                string managerResponse = _am.ValidateAndUpdateSession(token);
                if(managerResponse == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent("Request unauthorized",
                    Encoding.Unicode);
                }
                else
                {
                    if(headers.Contains("minLng") && headers.Contains("maxLng") && 
                        headers.Contains("minLat") && headers.Contains("maxLat"))
                    {
                        try
                        {
                            float minLng = float.Parse(headers.GetValues("minLng").First());
                            float minLat = float.Parse(headers.GetValues("minLat").First());
                            float maxLng = float.Parse(headers.GetValues("maxLng").First());
                            float maxLat = float.Parse(headers.GetValues("maxLat").First());
                            var pointList = _pm.getAllPoints(minLat, minLng, maxLat, maxLng);
                            
                            response = Request.CreateResponse(HttpStatusCode.OK);
                            if (pointList != null)
                            {
                                var jsonContent = new JavaScriptSerializer().Serialize(pointList);
                                //Console.WriteLine("Points as Json String:\t", jsonContent.ToString());
                                response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            response = Request.CreateResponse(HttpStatusCode.BadRequest);
                            response.Content = new StringContent("Invalid field formatting",
                            Encoding.Unicode);
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest);
                        response.Content = new StringContent("Request Missing Required Fields",
                        Encoding.Unicode);
                    }
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("Request unauthorized",
                Encoding.Unicode);
            }
            
            return response;

        }
    }
}
