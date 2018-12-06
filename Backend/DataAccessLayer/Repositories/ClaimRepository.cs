using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ClaimRepository
    {
      
        public Service GetService(string claimName)
        {
            using (var _db = new DatabaseContext())
            {
                Service service = _db.Services
                    .Where(c => c.ServiceName == claimName)
                    .FirstOrDefault();
                return service;
            }
        }

        public void AddServiceToUser(Guid userId, Guid serviceId)
        {
            using (var _db = new DatabaseContext())
            {
                var u = new Claim
                {
                    UserId = userId,
                    ServiceId = serviceId,
                };
                _db.Claims.Add(u);
                _db.SaveChanges();
            }
        }

        public bool UserHasServiceAccess(Guid userId, Guid serviceId)
        {
            using (var _db = new DatabaseContext())
            {
                int count = _db.Claims
                    .Where(c => c.UserId == userId && c.ServiceId == serviceId)
                    .Count();

                return count > 0;
            }
        }

        public bool CanUserMakeActionOnUser(Service service, User user1, User user2)
        {
            using (var _db = new DatabaseContext())
            {
                var count = _db.Claims
                    .Where(c => c.UserId == user1.Id && c.SubjectUserId == user2.Id && c.ServiceId == service.Id)
                    .Count();

                return count > 0;
            }
        }
        public bool IsServiceDisabled(Service service)
        {
            using (var _db = new DatabaseContext())
            {
                return _db.Services.Find(service.Id).Disabled;
            }
        }
        public void ToggleFeature(Service service, bool toggle)
        {
            service.UpdatedAt = DateTime.UtcNow;
            using (var _db = new DatabaseContext())
            {
                try
                {
                    _db.Entry(service).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
