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
        public int CreatePoint(DatabaseContext _db, Point point)
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

        public Point GetPoint(string pointName)
        {
            using (var _db = new DatabaseContext())
            {
                Point point = _db.Points
                    .Where(p => p.Name == pointName)
                    .FirstOrDefault();
                return point;
            }
        }

        public Point UpdatePoint(Point point)
        {
            using (var _db = new DatabaseContext())
            {

            }
        }
    }
}
