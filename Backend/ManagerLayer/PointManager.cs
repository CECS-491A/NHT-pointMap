using ServiceLayer.Services;
using DataAccessLayer.Database;
using DataAccessLayer.Models;

namespace ManagerLayer
{
    class PointManager
    {
        DatabaseContext _db;
        PointService _ps;

        public PointManager()
        {
            _db = new DatabaseContext();
            _ps = new PointService();
        }

        public Point CreatePoint(float longitude, float latitude, string description)
        {
            Point point = new Point();
            point.Description = description;
            point.Longitude = longitude;
            point.Latitude = latitude;

            return _ps.CreatePoint(_db, point);
        }

        public Point GetPoint(Point point)
        {
            return _ps.GetPoint(_db, point.Id);
        }

        public Point UpdatePoint(Point point)
        {
            return _ps.UpdatePoint(_db, point);
        }

        public Point DeletePoint(Point point)
        {
            return _ps.DeletePoint(_db, point.Id);
        }
    }
}
