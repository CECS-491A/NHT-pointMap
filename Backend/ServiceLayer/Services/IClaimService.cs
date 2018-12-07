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
        Service getService(string claimName);
        void addServiceToUser(User user, Service service, User subjectUser);
        bool userHasServiceAccess(User user, Service service, User subjectUser);
    }
}
