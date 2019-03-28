using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class ClaimService : IClaimService
    {
        private ClaimRepository _ClaimRepo;

        public ClaimService()
        {
            _ClaimRepo = new ClaimRepository();
        }

        public Claim CreateClaim(DatabaseContext _db, Claim claim)
        {
            return _ClaimRepo.CreateClaim(_db, claim);
        }

        public Claim CreateClaim(DatabaseContext _db, Guid userId, Guid serviceId)
        {
            Claim claim = new Claim
            {
                UserId = userId,
                ServiceId = serviceId,
                UpdatedAt = DateTime.UtcNow
            };
            return _ClaimRepo.CreateClaim(_db, claim);
        }

        public Service GetService(DatabaseContext _db, string claimName)
        {
            return _ClaimRepo.GetService(_db, claimName);
        }

        public void AddServiceToUser(DatabaseContext _db, User user, Service service)
        {
            _ClaimRepo.AddServiceToUser(_db, user.Id, service.Id);
        }

        public bool UserHasServiceAccess(DatabaseContext _db, User user, Service service)
        {
            if (service.Disabled) return false;
            return _ClaimRepo.UserHasServiceAccess(_db, user.Id, service.Id);
        }
    }
}
