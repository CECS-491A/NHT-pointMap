using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ClaimRepository
    {
        public Claim CreateClaim(DatabaseContext _db, Claim claim)
        {
            claim.UpdatedAt = DateTime.UtcNow;

            _db.Claims.Add(claim);

            return claim;
        }
      
        public Service GetService(DatabaseContext _db, string claimName)
        {
            Service service = _db.Services
                .Where(c => c.ServiceName == claimName)
                .FirstOrDefault();
            return service;
        }

        public void AddServiceToUser(DatabaseContext _db, Guid userId, Guid serviceId)
        {
            var u = new Claim
            {
                UserId = userId,
                ServiceId = serviceId
            };
            _db.Claims.Add(u);
        }

        public bool UserHasServiceAccess(DatabaseContext _db, Guid userId, Guid serviceId)
        {
            int count = _db.Claims
                .Where(c => c.UserId == userId && c.ServiceId == serviceId)
                .Count();

            return count > 0;
        }
    }
}
