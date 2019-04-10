using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ManagerLayer.AccessControl;
using System.Text;
using DataAccessLayer.Database;
using WebApi_PointMap.ErrorHandling;

namespace WebApi_PointMap.Controllers
{
    public class SessionController : ApiController
    {
        AuthorizationManager _am;
        DatabaseContext _db;

        public SessionController()
        {
            _am = new AuthorizationManager();
            _db = new DatabaseContext();
        }

        [HttpGet]
        [Route("api/session")]
        public HttpResponseMessage ValidateSession()
        {
            HttpResponseMessage response;
            var re = Request;
            var headers = re.Headers;
            if (headers.Contains("token"))
            {
                try
                {
                    string token = headers.GetValues("token").First();
                    var session = _am.ValidateAndUpdateSession(_db, token);
                    //TODO: deal with null session
                    _db.SaveChanges();
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(token,
                    Encoding.Unicode);
                    return response;
                }
                catch(Exception e)
                {
                    return LocalErrorHandler.HandleDatabaseException(e, _db);
                }
            }
            response = Request.CreateResponse(HttpStatusCode.Unauthorized);
            response.Content = new StringContent("https://kfc-sso.com/#/login",
            Encoding.Unicode);
            return response;
        }

        [HttpGet]
        [Route("api/logout/session")]
        public HttpResponseMessage DeleteSession()
        {
            HttpResponseMessage response;
            var re = Request;
            var headers = re.Headers;
            if (headers.Contains("token"))
            {
                try
                {
                    string token = headers.GetValues("token").First();
                    var session = _am.ValidateAndUpdateSession(_db, token);
                    if (session == null)
                    {
                        //return 404? (no session found)
                    }
                    _am.DeleteSession(_db, token);
                    _db.SaveChanges();

                    token = _am.DeleteSession(_db, token);
                    //TODO: handle null response
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(token,
                    Encoding.Unicode);
                }
                catch (Exception e)
                {
                    return LocalErrorHandler.HandleDatabaseException(e, _db);
                }
            }

            response = Request.CreateResponse(HttpStatusCode.Unauthorized);
            response.Content = new StringContent("https://kfc-sso.com/#/login",
            Encoding.Unicode);
            return response;
        }
    }
}
