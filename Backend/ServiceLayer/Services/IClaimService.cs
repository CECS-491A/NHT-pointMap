using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    interface IClaimService
    {
        Service GetService(string claimName);
        void AddServiceToUser(User user, Service service);
        bool UserHasServiceAccess(User user, Service service);
        void ToggleFeature(Service service, bool toggle);
        bool IsServiceEnabled(Service service);
        bool CanUserMakeActionOnUser(Service service, User user1, User user2);
    }
}
