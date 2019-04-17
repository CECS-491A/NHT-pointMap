using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.ErrorHandling
{
    public class HttpErrorHandler
    {
        public static HttpResponseMessage HandleException(Exception e)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            if (e is InvalidModelPayloadException)
            {
                httpResponse.StatusCode = HttpStatusCode.PreconditionFailed;
                httpResponse.Content = new StringContent(e.Message);
            }
            else if (e is InvalidHeaderException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(e.Message);
            }
            else
            {
                httpResponse = AuthorizationErrorHandler.HandleException(e);
            }
            return httpResponse;
        }
    }
}