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
    public class GeneralErrorHandler
    {
        public static HttpResponseMessage HandleException(Exception e)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            if (e is InvalidGuidException || e is InvalidEmailException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(e.Message);
            }
            else if(e is UserNotFoundException || e is PointNotFoundException)
            {
                httpResponse.StatusCode = HttpStatusCode.NotFound;
                httpResponse.Content = new StringContent(e.Message);
            }
            else
            {
                throw new Exception(e.Message, e);
            }
            return httpResponse;
        }
    }
}