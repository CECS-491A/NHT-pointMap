using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DataAccessLayer.Repositories
{
    public class PointRepository
    {
        private DatabaseContext _db;

        public PointRepository(DatabaseContext db)
        {
            _db = db;
        }
        private void ValidateLongLat(Point point)
        {
            if (point.Longitude > 180 || point.Longitude < -180 ||
                point.Latitude > 90 || point.Latitude < -90)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public Point CreatePoint(Point point)
        {
            ValidateLongLat(point);

            point.CreatedAt = DateTime.UtcNow;
            point.UpdatedAt = DateTime.UtcNow;
            var pointResponse =  _db.Points.Add(point);
            return pointResponse;
        }

        public Point GetPoint(float longitude, float latitude)
        {
            var point = _db.Points
                .Where(p => p.Latitude == latitude && p.Longitude == longitude)
                .FirstOrDefault<Point>();
            return point;
        }

        public Point GetPoint(Guid Id)
        {
            var point = _db.Points.Find(Id);
            return point;
        }

        public Point UpdatePoint(Point point)
        {
            ValidateLongLat(point);

            var pointToUpdate = _db.Points.Find(point.Id);
            if (pointToUpdate != null)
            {
                pointToUpdate.Name = point.Name;
                pointToUpdate.Description = point.Description;
                pointToUpdate.Latitude = point.Latitude;
                pointToUpdate.Longitude = point.Longitude;
                pointToUpdate.UpdatedAt = DateTime.UtcNow;
            }

            return pointToUpdate;
        }

        public Point DeletePoint(Guid Id)
        {
            Point point = _db.Points
                .Where(p => p.Id == Id)
                .FirstOrDefault<Point>();
            if (point == null)
            {
                return null;
            }
            point.UpdatedAt = DateTime.UtcNow;
            _db.Points.Remove(point);
            return point;
        }

        public List<Point> GetAllPoints(float minLat, float minLng, float maxLat, float maxLng)
        {
            List<Point> points = _db.Points
                .Where(p => (p.Latitude >= minLat && p.Latitude <= maxLat &&
                p.Longitude >= minLng && p.Longitude <= maxLng)).ToList();

            return points;
        }
    }
}
