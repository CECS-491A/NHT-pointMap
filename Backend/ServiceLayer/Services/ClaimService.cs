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
        ClaimRepository _ClaimRepo;

        public ClaimService()
        {
            _ClaimRepo = new ClaimRepository();
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
