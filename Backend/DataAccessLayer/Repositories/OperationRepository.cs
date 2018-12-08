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
            try
            {
                Service service = new Service
                {
                    ServiceName = serviceName,
                    Disabled = false
                };

                _db.Services.Add(service);
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int disableService(Guid id, DatabaseContext _db)
        {
            try
            {
                Service service = _db.Services.Find(id);
                //service.Disabled = true;
                //service.UpdatedAt = DateTime.UtcNow;
                //_db.Entry(service).State = EntityState.Modified;
                return 1;
            }
            catch(Exception)
            {
                return 0;
            }
        }

        public int enableService(Guid id, DatabaseContext _db)
        {
            try
            {
                Service service = _db.Services
                    .Where(s => s.Id == id)
                    .FirstOrDefault();
                service.Disabled = false;
                service.UpdatedAt = DateTime.UtcNow;
                _db.Entry(service).State = EntityState.Modified;
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
