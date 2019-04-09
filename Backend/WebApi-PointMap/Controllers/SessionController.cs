using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ManagerLayer.AccessControl;
using System.Text;

namespace WebApi_PointMap.Controllers
{
    public class SessionController : ApiController
    {
        AuthorizationManager _am;

        public SessionController()
        {
            _am = new AuthorizationManager();
        }

        [HttpGet]
        [Route("api/session")]
        public HttpResponseMessage validateSession()
        {
            HttpResponseMessage response;
            var re = Request;
            var headers = re.Headers;
            if (headers.Contains("token"))
            {
                string token = headers.GetValues("token").First();
                string managerResponse = _am.ValidateAndUpdateSession(token);
                if (managerResponse == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent("https://kfc-sso.com/#/login",
                    Encoding.Unicode);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(token,
                    Encoding.Unicode);
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("https://kfc-sso.com/#/login",
                Encoding.Unicode);
            }
            return response;
        }

        [HttpGet]
        [Route("api/logout/session")]
        public HttpResponseMessage deleteSession()
        {
            HttpResponseMessage response;
            var re = Request;
            var headers = re.Headers;
            if (headers.Contains("token"))
            {
                string token = headers.GetValues("token").First();
                string managerResponse = _am.ValidateAndUpdateSession(token);
                if (managerResponse == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent("https://kfc-sso.com/#/login",
                    Encoding.Unicode);
                }
                else
                {
                    try
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK);
                        token = _am.DeleteSession(token);
                        response.Content = new StringContent(token,
                        Encoding.Unicode);
                    }catch(Exception e)
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest);
                        response.Content = new StringContent("Invalid operation",
                        Encoding.Unicode);
                    }
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("https://kfc-sso.com/#/login",
                Encoding.Unicode);
            }
            return response;
        }
    }
}
