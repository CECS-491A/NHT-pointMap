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
    }
}
