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

        public Service GetService(string claimName)
        {
            return _ClaimRepo.GetService(claimName);
        }

        public void AddServiceToUser(User user, Service service)
        {
            _ClaimRepo.AddServiceToUser(user.Id, service.Id);
        }

        public bool UserHasServiceAccess(User user, Service service)
        {
            if (service.Disabled) return false;
            return _ClaimRepo.UserHasServiceAccess(user.Id, service.Id);
        }
        public bool CanUserMakeActionOnUser(Service service, User user1, User user2)
        {
            return _ClaimRepo.CanUserMakeActionOnUser(service, user1, user2);
        }

        public void ToggleFeature(Service service, bool toggle)
        {
            _ClaimRepo.ToggleFeature(service, toggle);
        }

        public bool IsServiceEnabled(Service service)
        {
            return _ClaimRepo.IsServiceDisabled(service);
        }
    }
}
