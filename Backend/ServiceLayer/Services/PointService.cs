using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System.Collections.Generic;

namespace ServiceLayer.Services
{
    public class PointService : IPointService
    {
        private PointRepository _PointRepo;

        public PointService()
        {
            _PointRepo = new PointRepository();
        }

        public Point CreatePoint(DatabaseContext _db, Point point)
        {
            return _PointRepo.CreatePoint(_db, point);
        }

        public Point DeletePoint(DatabaseContext _db, Guid Id)
        {
            return _PointRepo.DeletePoint(_db, Id);
        }

        public Point GetPoint(DatabaseContext _db, float longitude, float latitude)
        {
            return _PointRepo.GetPoint(_db, longitude, latitude);
        }

        public Point GetPoint(DatabaseContext _db, Guid Id)
        {
            return _PointRepo.GetPoint(_db, Id);
        }

        public Point UpdatePoint(DatabaseContext _db, Point point)
        {
            return _PointRepo.UpdatePoint(_db, point);
        }

        public List<Point> getAllPoints(DatabaseContext _db, float minLat, float minLng, float maxLat, float maxLng)
        {
            return _PointRepo.GetAllPoints(_db, minLat, minLng, maxLat, maxLng);
        }
    }
}
