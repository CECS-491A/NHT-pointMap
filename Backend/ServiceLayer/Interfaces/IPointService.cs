using System;
using DataAccessLayer.Database;
using DataAccessLayer.Models;

namespace ServiceLayer.Services
{
    public interface IPointService
    {
        Point CreatePoint(Point point);
        Point DeletePoint(Guid Id);
        Point GetPoint(Guid Id);
        Point UpdatePoint(Point point);
    }
}
