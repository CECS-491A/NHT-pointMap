using DataAccessLayer.Database;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using DTO.DTO;
using System.Net.Http;

namespace WebApi_PointMap.ErrorHandling
{
    public class LocalErrorHandler
    {
        public static HttpResponseMessage HandleDatabaseException(Exception e, DatabaseContext _db)
        {
            if (e is DbUpdateException || e is DbEntityValidationException)
            {
                RevertDatabaseChanges(_db);

                var messageDTO = new RecoverableErrorDTO
                {
                    Message = "The database operation failed."
                };

                HttpResponseMessage httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                httpResponse.Content = new StringContent(messageDTO.Message);

                return httpResponse;
            }
            else if( e is ArgumentOutOfRangeException)
            {
                RevertDatabaseChanges(_db);

                var messageDTO = new RecoverableErrorDTO
                {
                    Message = "Longitude/Latitude value invalid."
                };

                HttpResponseMessage httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                httpResponse.Content = new StringContent(messageDTO.Message);

                return httpResponse;
            }
            else
            {
                throw new Exception("An unknown error occured", e);
            }
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