using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class PointRepository
    {
        private Point CheckPointLimits(Point point)
        {
            if (point.Longitude > 180 || point.Longitude < -180 ||
                point.Latitude > 90 || point.Latitude < -90)
                return null;
            return point;
        }

        public bool ExistingPoint(DatabaseContext _db, Point point)
        {
            var result = GetPoint(_db, point.Id);
            if (result != null)
            {
                return true;
            }
            return false;
        }
        public Point CreatePoint(DatabaseContext _db, Point point)
        {
            if (CheckPointLimits(point) == null)
                return null;
            point.UpdatedAt = DateTime.UtcNow;
                _db.Points.Add(point);
            return point;
        }

        public Point GetPoint(DatabaseContext _db, float longitude, float latitude)
        {
            var point = _db.Points
                .Where(p => p.Latitude == latitude && p.Longitude == longitude)
                .FirstOrDefault<Point>();
            return point;
        }

        public Point GetPoint(DatabaseContext _db, Guid Id)
        {
            return _db.Points.Find(Id);
        }

        public Point UpdatePoint(DatabaseContext _db, Point point)
        {
            if (CheckPointLimits(point) == null)
                return null;

            point.UpdatedAt = DateTime.UtcNow;
            _db.Entry(point).State = EntityState.Modified;
            return point;
        }

        public Point DeletePoint(DatabaseContext _db, Guid Id)
        {
            Point point = _db.Points
                .Where(p => p.Id == Id)
                .FirstOrDefault<Point>();
            if (point == null)
                return null;
            _db.Entry(point).State = EntityState.Deleted;
            return point;
        }
    }
}
