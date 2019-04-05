using ServiceLayer.Services;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System;

namespace ManagerLayer
{
    public class PointManager
    {
        PointService _ps;

        public PointManager()
        {
            _ps = new PointService();
        }

        public Point CreatePoint(DatabaseContext _db, float longitude, float latitude, string description, string name)
        {
            Point point = new Point();
            point.Description = description;
            point.Longitude = longitude;
            point.Latitude = latitude;
            point.Name = name;

            point = _ps.CreatePoint(_db, point);

            return point;
        }

        public Point GetPoint(DatabaseContext _db, Guid pointId)
        {
            return _ps.GetPoint(_db, pointId);
        }

        public Point UpdatePoint(DatabaseContext _db, Guid pointId, float longitude, float latitude, 
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
            return point;
        }

        public Point DeletePoint(DatabaseContext _db, Guid pointId)
        {
            Point point = _ps.DeletePoint(_db, pointId);

            return point;
        }
    }
}
