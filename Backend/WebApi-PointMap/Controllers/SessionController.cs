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
using DTO;
using DataAccessLayer.Models;
using Logging.Logging;

namespace WebApi_PointMap.Controllers
{
    public class SessionController : ApiController
    {
        AuthorizationManager _am;
        DatabaseContext _db;
        Logger logger;
        LogRequestDTO newLog;

        private Session session;

        public SessionController()
        {
            logger = new Logger();
            newLog = new LogRequestDTO();
        }

        [HttpGet]
        [Route("api/session")]
        public HttpResponseMessage ValidateSession()
        { 
            using (var _db = new DatabaseContext())
            {
                try
                {
                    var token = ControllerHelpers.GetToken(Request);
                    session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                    newLog = logger.initalizeAnalyticsLog("Validated and updated Session at SessionController line 35 for " +
                        "validate session", newLog.sessionSource, session.User, session);
                    logger.sendLogAsync(newLog); //Sends session log
                    _db.SaveChanges();
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(token, Encoding.Unicode);
                    return response;
                }
                catch (Exception e)
                {
                    logger.sendErrorLog(newLog.sessionSource, e.StackTrace, session.User.Id.ToString(),
                    session.User.Username, null, session);

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
                    session = ControllerHelpers.ValidateAndUpdateSession(_db, token);
                    newLog = logger.initalizeAnalyticsLog("Validated and updated Session at SessionController line 59 " +
                        "for logout", newLog.sessionSource, session.User, session);
                    logger.sendLogAsync(newLog); //Sends session log

                    _am.DeleteSession(_db, token);
                    _db.SaveChanges();

                    newLog = logger.initalizeAnalyticsLog("Deleted Session at SessionController line 66 for logout line 68\n" +
                        "route: api/logout/session", newLog.logoutSource, session.User, session);
                    logger.sendLogAsync(newLog); //Sends logout log

                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(ControllerHelpers.Redirect, Encoding.Unicode);

                    return response;
                }
                catch (Exception e)
                {
                    logger.sendErrorLog(newLog.logoutSource, e.StackTrace, session.User.Id.ToString(),
                    session.User.Username, null, session);

                    return DatabaseErrorHandler.HandleException(e, _db);
                }
            }
        }
    }
}
