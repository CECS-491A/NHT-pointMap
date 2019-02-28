using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class PointRepository
    {
        public int CreatePoint(Point point)
        {
            using (var _db = new DatabaseContext())
            {
                point.UpdatedAt = DateTime.UtcNow;
                try
                {
                    _db.Points.Add(point);
                    return _db.SaveChanges();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public Service GetService(string pointName)
        {
            using (var _db = new DatabaseContext())
            {
                Service service = _db.Services
                    .Where(c => c.ServiceName == pointName)
                    .FirstOrDefault();
                return service;
            }
        }
    }
}
