using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System.Collections.Generic;
using static ServiceLayer.Services.ExceptionService;

namespace ServiceLayer.Services
{
    public class PointService : IPointService
    {
        private PointRepository _PointRepo;
        private DatabaseContext _db;

        public PointService(DatabaseContext db)
        {
            _PointRepo = new PointRepository(db);
            _db = db;
        }

        public Point CreatePoint(Point point)
        {
            try
            {
                var pointCreated = _PointRepo.CreatePoint(point);
                return pointCreated;
            }
            catch(ArgumentOutOfRangeException e)
            {
                throw new InvalidPointException("Longitude/Latitude value invalid.", e);
            }
        }

        public Point DeletePoint(Guid Id)
        {
            var point = _PointRepo.DeletePoint(Id);
            return point;
        }

        public Point GetPoint(Guid Id)
        {
           var point = _PointRepo.GetPoint(Id);
            return point;
        }

        public Point UpdatePoint(Point point)
        {
            try
            {
                var pointUpdated = _PointRepo.UpdatePoint(point);
                return pointUpdated;
            }
            catch (ArgumentOutOfRangeException e)
            {
                //catches error from DataAccessLayer and wraps it with a more specific error
                throw new InvalidPointException("Longitude/Latitude value invalid.", e);
            }
        }

        public List<Point> GetAllPoints(float minLat, float minLng, float maxLat, float maxLng)
        {
            return _PointRepo.GetAllPoints(minLat, minLng, maxLat, maxLng);
        }
    }
}
