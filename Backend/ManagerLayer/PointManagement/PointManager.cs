using ServiceLayer.Services;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;

namespace ManagerLayer
{
    public class PointManager
    {
        DatabaseContext _db;
        PointService _ps;

        public PointManager()
        {
            _ps = new PointService();
        }

        public Point CreatePoint(float longitude, float latitude, string description, string name)
        {
            Point point = new Point();
            point.Description = description;
            point.Longitude = longitude;
            point.Latitude = latitude;
            point.Name = name;

            point = _ps.CreatePoint(_db, point);
            _db.SaveChanges();

            return point;
        }

        public Point GetPoint(Guid pointId)
        {
            return _ps.GetPoint(_db, pointId);
        }

        public Point UpdatePoint(Guid pointId, float longitude, float latitude, 
                                string description, string name, DateTime createdAt)
        {
            Point point = new Point
            {
                CreatedAt = createdAt,
                Id = pointId,
                Longitude = longitude,
                Latitude = latitude,
                Description = description,
                Name = name
            };

            point = _ps.UpdatePoint(_db, point);
            _db.SaveChanges();
            return point;
        }

        public Point DeletePoint(Guid pointId)
        {
            _db = new DatabaseContext();
            Point point = _ps.DeletePoint(_db, pointId);
            _db.SaveChanges();
            return point;
        }

        public List<Point> getAllPoints(float minLat, float minLng, float maxLat, float maxLng)
        {
            _db = new DatabaseContext();
            return _ps.getAllPoints(_db, minLat, minLng, maxLat, maxLng);
        }
    }
}
