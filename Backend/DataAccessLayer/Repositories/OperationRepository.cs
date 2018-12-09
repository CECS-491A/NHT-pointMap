using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using System.Data.Entity;
using DataAccessLayer.Database;

namespace DataAccessLayer.Repositories
{
    public class OperationRepository
    {

        public int createService(Service service, DatabaseContext _db)
        {
            try
            {
                if (service == null || service.Id == null || service.ServiceName == null)
                    return -1;
                _db.Services.Add(service);
                return 1;

            } 
            catch(Exception)
            {
                return 0;
            }
        }

        public int createService(string serviceName, DatabaseContext _db)
        {
            Service service = new Service
            {
                ServiceName = serviceName,
                Disabled = false
            };

            return createService(service, _db);
        }

        public int deleteService(Guid id, DatabaseContext _db)
        {
            try
            {
                Service service = getService(id, _db);
                if (service == null || service.Id == null || service.ServiceName == null)
                    return -1;
                service.Disabled = true;
                service.UpdatedAt = DateTime.UtcNow;
                _db.Entry(service).State = EntityState.Deleted;
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int enableService(Guid id, DatabaseContext _db)
        {
            return toggleService(id, _db, false);
        }

        public int disableService(Guid id, DatabaseContext _db)
        {
            return toggleService(id, _db, true);
        }

        private int toggleService(Guid id, DatabaseContext _db, bool state)
        {
            try
            {
                Service service = getService(id, _db);
                if (service == null)
                    return -1;
                service.Disabled = state;
                service.UpdatedAt = DateTime.UtcNow;
                _db.Entry(service).State = EntityState.Modified;
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private Service getService(Guid id, DatabaseContext _db)
        {
            Service service = _db.Services
                    .Where(s => s.Id == id)
                    .FirstOrDefault();
            return service;
        }

    }
}
