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
        private AuthorizationManager _am;
        private DatabaseContext _db;

        public SessionController()
        {
            _db = new DatabaseContext();
        }

        [HttpGet]
        [Route("api/session")]
        public HttpResponseMessage ValidateSession()
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request);
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                _db.SaveChanges();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(token, Encoding.Unicode);
                return response;
            }
            catch (Exception e)
            {
                return DatabaseErrorHandler.HandleException(e, _db);
            }
        }

        [HttpGet]
        [Route("api/logout/session")]
        public HttpResponseMessage DeleteSession()
        {
            _am = new AuthorizationManager(_db);
            try
            {
                var token = ControllerHelpers.GetToken(Request);
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                _am.DeleteSession(token);
                _db.SaveChanges();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(ControllerHelpers.Redirect, Encoding.Unicode);

                return response;
            }
            catch (Exception e)
            {
                return DatabaseErrorHandler.HandleException(e, _db);
            }
        }
    }
}
