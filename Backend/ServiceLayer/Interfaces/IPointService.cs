using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;

namespace ServiceLayer.Services
{
    public interface IPointService
    {
        Point CreatePoint(Point point);
        Point DeletePoint(Guid Id);
        Point GetPoint(float longitude, float latitude);
        Point GetPoint(Guid Id);
        Point UpdatePoint(Point point);
    }
}
