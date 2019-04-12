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
        public IHttpActionResult ValidateSession()
        {
            try
            { 
                var token = ControllerHelpers.GetToken(Request, "Token");
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                _db.SaveChanges();
                return Ok(new StringContent(token, Encoding.Unicode));
            }
            catch(Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }

        [HttpGet]
        [Route("api/logout/session")]
        public IHttpActionResult DeleteSession()
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request, "Token");
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                _am.DeleteSession(_db, token);

                _am.DeleteSession(_db, token);
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return ResponseMessage(DatabaseErrorHandler.HandleException(e, _db));
            }
        }
    }
}
