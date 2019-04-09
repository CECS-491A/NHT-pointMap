using DataAccessLayer.Database;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Net.Http;
using System.Net;

namespace WebApi_PointMap.ErrorHandling
{
    public class LocalErrorHandler
    {
        public static HttpResponseMessage HandleDatabaseException(Exception e, DatabaseContext _db)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            if (e is DbUpdateException || e is DbEntityValidationException)
            {
                RevertDatabaseChanges(_db);

                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent("The database operation failed.");
            }
            else if( e is ArgumentOutOfRangeException)
            {
                RevertDatabaseChanges(_db);

                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Longitude/Latitude value invalid.");
            }
            else
            {
                throw new Exception("An unknown error occured", e);
            }
            return httpResponse;
        }

        private static void RevertDatabaseChanges(DatabaseContext _db)
        {
            foreach(var entry in _db.ChangeTracker.Entries())
            {
                switch(entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
    }
}