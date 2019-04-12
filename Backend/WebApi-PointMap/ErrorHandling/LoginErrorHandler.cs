﻿using DataAccessLayer.Database;
using System;
using System.Net;
using System.Net.Http;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.ErrorHandling
{
    public class LoginErrorHandler
    {
        public static HttpResponseMessage HandleDatabaseException(Exception e, DatabaseContext _db)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            if (e is FormatException || e is InvalidEmailException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Invalid credentials.");
            }
            else if (e is InvalidTokenSignatureException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                httpResponse.Content = new StringContent("Login failed.");
            }
            else
            {
                httpResponse = DatabaseErrorHandler.HandleException(e, _db);
            }
            return httpResponse;
        }
    }
}