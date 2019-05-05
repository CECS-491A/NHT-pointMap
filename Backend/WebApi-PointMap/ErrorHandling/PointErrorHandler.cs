﻿using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.ErrorHandling
{
    public class PointErrorHandler
    {
        public static HttpResponseMessage HandleException(Exception e)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            if (e is InvalidPointException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(e.Message);
            }
            return httpResponse;
        }
    }
}