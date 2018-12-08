using System;
using System.Collections.Generic;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using DataAccessLayer.Repositories;

namespace ServiceLayer.Services
{
    public class OperationService : IOperationService
    {
        public OperationService()
        { }

        public int createService(Service service, DatabaseContext _db)
        {
            OperationRepository _or = new OperationRepository();
            return _or.createService(service, _db);
        }

        public int createService(string serviceName, DatabaseContext _db)
        {
            OperationRepository _or = new OperationRepository();
            return _or.createService(serviceName, _db);
        }

        public int disableService(Guid serviceId, DatabaseContext _db)
        {
            OperationRepository _or = new OperationRepository();
            return _or.disableService(serviceId, _db);
        }

        public int enableService(Guid serviceId, DatabaseContext _db)
        {
            OperationRepository _or = new OperationRepository();
            return _or.enableService(serviceId, _db);
        }
        
    }
}
