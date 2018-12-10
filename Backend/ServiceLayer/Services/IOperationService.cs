using System;
using System.Collections.Generic;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using DataAccessLayer.Repositories;

namespace ServiceLayer.Services
{
    interface IOperationService
    {
        int createService(Service service, DatabaseContext _db);
        int createService(string serviceName, DatabaseContext _db);
        int disableService(Guid serviceId, DatabaseContext _db);
        int enableService(Guid serviceId, DatabaseContext _db);
        int deleteService(Guid serviceId, DatabaseContext _db);
    }
}
