using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.AccessControl
{
    class AuthorizationManager
    {
        private ISessionService _sessionService;

        private DatabaseContext CreateDbContext()
        {
            return new DatabaseContext();
        }

        public AuthorizationManager()
        {
             _sessionService = new SessionService();
        }
        public string GenerateSession(Guid userId)
        {
            using (var _db = CreateDbContext())
            {
                var response = _sessionService.GenerateSession(_db, userId);
                try
                {
                    _db.SaveChanges();
                    return response.Token;
                }
                catch (DbEntityValidationException ex)
                {
                    //catch error
                    // detach session attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
            }
            return null;
        }

        public string ValidateAndUpdateSession(string token, Guid userId)
        {
            using (var _db = CreateDbContext())
            {
                var response = _sessionService.ValidateSession(_db, token, userId);

                if(response != null)
                {
                    response = _sessionService.UpdateSession(_db, response);
                }
                else
                {
                    return null;
                }

                try
                {
                    _db.SaveChanges();
                    return response.Token;
                }
                catch (DbEntityValidationException ex)
                {
                    //catch error
                    // detach session attempted to be created from the db context - rollback
                    _db.Entry(response).State = System.Data.Entity.EntityState.Detached;
                }
            }
            return null;
        }
    }
}
