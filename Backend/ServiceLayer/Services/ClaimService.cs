using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class ClaimService : IClaimService
    {
        public Service getService(string claimName)
        {
            using (var _db = new DatabaseContext())
            {
                Service service = _db.Services
                    .Where(c => c.ServiceName == claimName)
                    .FirstOrDefault();

                return service;
            }
        }

        public void addServiceToUser(User user, Service service)
        {
            using (var _db = new DatabaseContext())
            {
                var u = new Claim
                {
                    Id = new Guid(),
                    UserId = user.Id,
                    ServiceId = service.Id,
                };
                _db.Claims.Add(u);
                _db.SaveChanges();
            }
        }

        public bool userHasServiceAccess(User user, Service service)
        {
            if (service.Disable) return false;
            
            using (var _db = new DatabaseContext())
            {
                int count = _db.Claims
                    .Where(c => c.UserId == user.Id && c.ServiceId == service.Id)
                    .Count();

                return count > 0;
            }
        }
    }
}
