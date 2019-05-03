using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using static ServiceLayer.Services.ExceptionService;
using System.ComponentModel.DataAnnotations;

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
            else if(e is UserNotFoundException)
            {
                httpResponse.StatusCode = HttpStatusCode.NotFound;
                httpResponse.Content = new StringContent(e.Message);
            }
<<<<<<< HEAD
            else if(e is ValidationException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(e.Message);
=======
            else if(e is UserAlreadyExistsException)
            {
                httpResponse.StatusCode = HttpStatusCode.Conflict;
                httpResponse.Content = new StringContent(e.Message);
>>>>>>> ba10c9942d47f8e170a95c16f4779a8e0ed0571c
            }
            else
            {
                throw new Exception(e.Message, e);
            }
            return httpResponse;
        }
    }
}