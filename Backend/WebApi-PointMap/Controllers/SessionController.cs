using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ManagerLayer.AccessControl;
using System.Text;
using DataAccessLayer.Database;
using static ServiceLayer.Services.ExceptionService;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

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
                var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                _db.SaveChanges();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(session.Token, Encoding.Unicode);
                return response;
            }
            catch (Exception e) when (e is NoTokenProvidedException ||
                                        e is SessionNotFoundException)
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized, e.Message);
                return response;
            }
            catch (Exception e)
            {
                if(e is DbUpdateException ||
                    e is DbEntityValidationException)
                {
                    _db.RevertDatabaseChanges(_db);
                }
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }

        [HttpGet]
        [Route("api/logout/session")]
        public HttpResponseMessage DeleteSession()
        {
            _am = new AuthorizationManager(_db);
            try
            {
                var session = ControllerHelpers.ValidateAndUpdateSession(Request);

                _am.DeleteSession(session.Token);
                _db.SaveChanges();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(ControllerHelpers.Redirect, Encoding.Unicode);

                return response;
            }
            catch (Exception e) when (e is NoTokenProvidedException ||
                                        e is SessionNotFoundException)
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized, e.Message);
                return response;
            }
            catch (Exception e)
            {
                if (e is DbUpdateException ||
                    e is DbEntityValidationException)
                {
                    _db.RevertDatabaseChanges(_db);
                }
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                return response;
            }
        }
    }
}
