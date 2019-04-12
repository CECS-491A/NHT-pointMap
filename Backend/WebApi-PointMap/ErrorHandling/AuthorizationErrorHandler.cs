using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
                httpResponse.Content = new StringContent("https://kfc-sso.com/#/login",
                Encoding.Unicode);
            }
            else if (e is NullReferenceException)
            {
                if(string.Equals(e.Message, "User does not exist."))
                {
                    httpResponse.StatusCode = HttpStatusCode.NotFound;
                    httpResponse.Content = new StringContent(e.Message);
                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                    httpResponse.Content = new StringContent("https://kfc-sso.com/#/login",
                    Encoding.Unicode);
                }
                
            }
            else if (e is InvalidTokenSignatureException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                httpResponse.Content = new StringContent(e.Message);
            }
            else
            {
                httpResponse = DatabaseErrorHandler.HandleException(e, _db);
            }
            return httpResponse;
        }
    }
}