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
using static ServiceLayer.Services.ExceptionService;
using ManagerLayer.Users;

namespace WebApi_PointMap.Controllers
{
    public class SessionController : ApiController
    {
        AuthorizationManager _am;

        [HttpGet]
        [Route("api/session")]
        public HttpResponseMessage ValidateSession()
        {
            using (var _db = new DatabaseContext())
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
                    var _userManager = new UserManager(_db);
                    _userManager.Logout(token);
                    _db.SaveChanges();
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(ControllerHelpers.Redirect, Encoding.Unicode);
                    return response;
                }
                catch (Exception e)
                {
                    if (e is NoTokenProvidedException)
                    {
                        return AuthorizationErrorHandler.HandleException(e);
                    }
                    return DatabaseErrorHandler.HandleException(e, _db);
                }
            }
        }
    }
}
