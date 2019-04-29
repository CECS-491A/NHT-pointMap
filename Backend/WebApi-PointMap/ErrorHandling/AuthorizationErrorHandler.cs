using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using WebApi_PointMap.Controllers;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.ErrorHandling
{
    public class AuthorizationErrorHandler
    {
        public static HttpResponseMessage HandleException(Exception e)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            if(e is InvalidModelPayloadException)
            {
                httpResponse.StatusCode = HttpStatusCode.PreconditionFailed;
                httpResponse.Content = new StringContent(e.Message);
            }
            else if (   e is NoTokenProvidedException || 
                        e is SessionNotFoundException ||
                        e is InvalidTokenSignatureException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                httpResponse.Content = new StringContent(e.Message);
            }
            else if(e is UserIsNotAdministratorException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                httpResponse.Content = new StringContent(e.Message);
            }
            else
            {
                httpResponse = GeneralErrorHandler.HandleException(e);
            }
            return httpResponse;
        }
    }
}