using DataAccessLayer.Database;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Net.Http;
using System.Net;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.ErrorHandling
{
    public class DatabaseErrorHandler
    {
        public static HttpResponseMessage HandleException(Exception e, DatabaseContext _db)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            if (e is DbUpdateException || e is DbEntityValidationException)
            {
                _db.RevertDatabaseChanges(_db);

                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent("The database operation failed.");
            }
            else
            {
                throw new Exception(e.Message, e);
            }
            return httpResponse;
        }
    }
}