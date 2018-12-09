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
        private ClientRepository _ClientRepo;

        public ClaimService()
        {
            _ClaimRepo = new ClaimRepository();
            _ClientRepo = new ClientRepository();
        }

        public int CreateClaim(Claim claim)
        {
            return _ClaimRepo.CreateClaim(claim);
        }

        public int CreateClaim(Guid userId, Guid serviceId)
        {
            Claim claim = new Claim
            {
                UserId = userId,
                ServiceId = serviceId,
                UpdatedAt = DateTime.UtcNow
            };
            return _ClaimRepo.CreateClaim(claim);
        }

        public Service getService(string claimName)
        {
            return _ClaimRepo.GetService(claimName);
        }

        public void addServiceToUser(User user, Service service)
        {
            _ClaimRepo.AddServiceToUser(user.Id, service.Id);
        }

        public bool userHasServiceAccess(User user, Service service)
        {
            if (service.Disabled) return false;
            return _ClaimRepo.UserHasServiceAccess(user.Id, service.Id);
        }
    }
}
