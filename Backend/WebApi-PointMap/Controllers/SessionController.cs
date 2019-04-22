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
using DataAccessLayer.Models;

namespace WebApi_PointMap.Controllers
{
    public class SessionController : ApiController
    {
        AuthorizationManager _am;
        DatabaseContext _db;
        LoggingManager _lm;
        LogRequestDTO newLog;

        [HttpGet]
        [Route("api/session")]
        public HttpResponseMessage ValidateSession()
        {
            _lm = new LoggingManager();
            newLog = new LogRequestDTO();
            using (var _db = new DatabaseContext())
            {
                try
                {
                    var token = ControllerHelpers.GetToken(Request);
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                    initalizeLogForController("Validated and updated Session at SessionController line 35 for " +
                        "validate session", true);
                    newLog = _lm.addSessionToLog(newLog, session);
                    _lm.sendLogAsync(newLog); //Sends session log
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
        }

        [HttpGet]
        [Route("api/logout/session")]
        public HttpResponseMessage DeleteSession()
        {
            using (var _db = new DatabaseContext())
            {
                _am = new AuthorizationManager(_db);
                try
                {
                    var token = ControllerHelpers.GetToken(Request);
                    var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                    initalizeLogForController("Validated and updated Session at SessionController line 59 " +
                        "for logout", true);
                    newLog = _lm.addSessionToLog(newLog, session);
                    _lm.sendLogAsync(newLog); //Sends session log

                    _am.DeleteSession(_db, token);
                    initalizeLogForController("Deleted Session at SessionController line 66 for logout", true);
                    newLog.page = newLog.logoutPage;
                    newLog = _lm.addSessionToLog(newLog, session);
                    _lm.sendLogAsync(newLog); //Sends logout log
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

        private void initalizeLogForController(string details, bool success)
        {
            newLog = new LogRequestDTO();
            newLog.source = "Session Controller";
            newLog.details = details;
            newLog.success = success;
            newLog.page = newLog.sessionPage;
        }
    }
}
