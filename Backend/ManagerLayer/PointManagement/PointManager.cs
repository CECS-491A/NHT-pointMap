using ServiceLayer.Services;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System;
using System.Collections.Generic;

namespace ManagerLayer
{
    public class PointManager
    {
        PointService _ps;
        DatabaseContext _db;

        public PointManager(DatabaseContext db)
        {
            _ps = new PointService(db);
            _db = db;
        }

        public Point CreatePoint(float longitude, float latitude, string description, string name)
        {
            var point = new Point
            {
                Description = description,
                Longitude = longitude,
                Latitude = latitude,
                Name = name
            };

            point = _ps.CreatePoint(point);

            return point;
        }

        public Point GetPoint(Guid pointId)
        {
            return _ps.GetPoint(pointId);
        }

        public Point UpdatePoint(Guid pointId, float longitude, float latitude, 
                                string description, string name, DateTime createdAt)
        {
            var point = new Point
            {
                CreatedAt = createdAt,
                Id = pointId,
                Longitude = longitude,
                Latitude = latitude,
                Description = description,
                Name = name
            };

            point = _ps.UpdatePoint(point);
            return point;
        }

        public Point DeletePoint(Guid pointId)
        {
            var point = _ps.DeletePoint(pointId);

            return point;
        }

        public List<Point> GetAllPoints(float minLat, float minLng, float maxLat, float maxLng)
        {
            return _ps.GetAllPoints(minLat, minLng, maxLat, maxLng);
        }
    }
}
