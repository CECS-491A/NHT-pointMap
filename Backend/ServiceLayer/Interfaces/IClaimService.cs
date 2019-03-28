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
        Claim CreateClaim(DatabaseContext _db, Claim claim);
        Claim CreateClaim(DatabaseContext _db, Guid userId, Guid serviceId);
        Service GetService(DatabaseContext _db, string claimName);
        void AddServiceToUser(DatabaseContext _db, User user, Service service);
        bool UserHasServiceAccess(DatabaseContext _db, User user, Service service);
    }
}
