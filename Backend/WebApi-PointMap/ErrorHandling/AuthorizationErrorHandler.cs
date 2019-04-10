using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.ErrorHandling
{
    public class AuthorizationErrorHandler
    {
        public static HttpResponseMessage HandleException(Exception e, DatabaseContext _db)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            if (e is FormatException)
            {
                if (string.Equals(e.Message, "Invalid payload."))
                {
                    httpResponse.StatusCode = HttpStatusCode.PreconditionFailed;
                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                }

                httpResponse.Content = new StringContent(e.Message);
            }
            else if (e is HttpRequestException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                httpResponse.Content = new StringContent(e.Message);
            }
            else if (e is NullReferenceException)
            {
                httpResponse.StatusCode = HttpStatusCode.NotFound;
                httpResponse.Content = new StringContent(e.Message);
            }
            else if (e is InvalidTokenSignatureException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                httpResponse.Content = new StringContent(e.Message);
            }
            else
            {
                httpResponse = LocalErrorHandler.HandleDatabaseException(e, _db);
            }
            return httpResponse;
        }
    }
}