using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;

namespace ServiceLayer.Services
{
    public interface IPointService
    {
        Point CreatePoint(DatabaseContext _db, Point point);
        Point DeletePoint(DatabaseContext _db, Guid Id);
        Point GetPoint(DatabaseContext _db, float longitude, float latitude);
        Point GetPoint(DatabaseContext _db, Guid Id);
        Point UpdatePoint(DatabaseContext _db, Point point);
    }
}
