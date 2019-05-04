using DataAccessLayer.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.ErrorHandling
{
    /*
     * Add any new exceptions that need to be handled in the appropriate commented category below
     */
    public class ErrorHandler
    {
        public static HttpResponseMessage HandleException(Exception e, DatabaseContext _db)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.Content = new StringContent(e.Message);

            //Authorization Exceptions
            if (e is InvalidModelPayloadException || e is ArgumentNullException)
            {
                httpResponse.StatusCode = HttpStatusCode.PreconditionFailed;
            }
            else if (e is NoTokenProvidedException ||
                     e is SessionNotFoundException ||
                     e is InvalidTokenSignatureException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
            }
            else if (e is UserIsNotAdministratorException)
            {
                httpResponse.StatusCode = HttpStatusCode.Unauthorized;
            }
            //Http Exception
            else if (e is InvalidModelPayloadException)
            {
                httpResponse.StatusCode = HttpStatusCode.PreconditionFailed;
            }
            //SSO Exception
            else if(e is KFCSSOAPIRequestException)
            {
                httpResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
            }
            //Database Exceptions
            else if (e is InvalidHeaderException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
            }
            else if (e is DbUpdateException || e is DbEntityValidationException)
            {
                _db.RevertDatabaseChanges(_db);

                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent("The database operation failed.");
            }
            //Point Exception
            else  if (e is InvalidPointException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
            }
            //General Exceptions
            else if (e is InvalidGuidException || e is InvalidEmailException)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
            }
            else if(e is UserNotFoundException)
            {
                httpResponse.StatusCode = HttpStatusCode.NotFound;
            }
            else if(e is UserAlreadyExistsException)
            {
                httpResponse.StatusCode = HttpStatusCode.Conflict;
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
            }
            return httpResponse;
        }
    }
}