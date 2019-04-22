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
using ManagerLayer.Logging;
using DTO;

namespace WebApi_PointMap.Controllers
{
    public class SessionController : ApiController
    {
        AuthorizationManager _am;
        DatabaseContext _db;
        LoggingManager _lm;
        LogRequestDTO newLog;

        public SessionController()
        {
            _am = new AuthorizationManager();
            _db = new DatabaseContext();
            _lm = new LoggingManager();
            newLog = new LogRequestDTO();
        }

        [HttpGet]
        [Route("api/session")]
        public HttpResponseMessage ValidateSession()
        {
            try
            { 
                var token = ControllerHelpers.GetToken(Request);
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                newLog.source = "Session";
                newLog.details = "Session successful validation at SessionController line 40";
                newLog.token = session.Token;
                newLog.success = true;
                newLog.page = "Session";
                newLog.sessionExpiredAt = session.ExpiresAt;
                newLog.sessionCreatedAt = session.CreatedAt;
                newLog.sessionUpdatedAt = session.UpdatedAt;
                _lm.sendLogAsync(newLog);
                _db.SaveChanges();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(token, Encoding.Unicode);
                return response;
            }
            catch(Exception e)
            {
                return DatabaseErrorHandler.HandleException(e, _db);
            }
        }

        [HttpGet]
        [Route("api/logout/session")]
        public HttpResponseMessage DeleteSession()
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request);
                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                _am.DeleteSession(_db, token);
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
