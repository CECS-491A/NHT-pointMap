using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using System.Data.Entity;
using System.Linq;

namespace UnitTesting
{
    [TestClass]
    public class OperationServiceUT
    {
        OperationService _os;
        Service service1;
        Service service2;
        TestingUtils _ts;

        public OperationServiceUT()
        {
            _os = new OperationService();
            _ts = new TestingUtils();
        }

        [TestMethod]
        public void createServiceObject()
        {
            service1 = new Service
            {
                ServiceName = "" + Guid.NewGuid(),
                Disabled = false
                
            };
            using (var _db = new DatabaseContext())
            {
                //Checking if service was created
                Assert.AreEqual(1, _os.createService(service1, _db));
                service2 = _db.Services.Find(service1.Id);
                //Ensuring data integrity of service
                Assert.AreEqual(service1.ServiceName, service2.ServiceName);
            }
        }

        [TestMethod]
        public void createServiceString()
        {
            string serviceName = "" + Guid.NewGuid();
            using (var _db = new DatabaseContext())
            {
                //Checking if service was created
                var response = _os.createService(serviceName, _db);
                Assert.AreEqual(1, response);
                _db.SaveChanges();
                Service service = _db.Services
                    .Where(s => s.ServiceName == serviceName)
                    .FirstOrDefault();
                //Ensuring data integrity of service
                Assert.IsNotNull(service);
                Assert.AreEqual(serviceName, service.ServiceName);
            }
        }

        [TestMethod]
        public void disableService()
        {
            service1 = _ts.CreateServiceInDb(true);

            using (var _db = new DatabaseContext())
            {
                Assert.AreEqual(false, service1.Disabled);
                Assert.AreEqual(1, _os.disableService(service1.Id, _db));
                _db.SaveChanges();
                Service service = _db.Services
                    .Where(s => s.Id == service1.Id)
                    .FirstOrDefault();
                Assert.AreEqual(true, service.Disabled);
            }
        }

        [TestMethod]
        public void enableService()
        {
            Service service1 = _ts.CreateServiceInDb(false);
            
            using (var _db = new DatabaseContext())
            {
                Assert.AreEqual(true, service1.Disabled);
                Assert.AreEqual(1, _os.enableService(service1.Id, _db));
                _db.SaveChanges();
                Service service = _db.Services
                    .Where(s => s.Id == service1.Id)
                    .FirstOrDefault();
                Assert.AreEqual(false, service.Disabled);
            }
        }

        [TestMethod]
        public void deleteService()
        {
            service1 = _ts.CreateServiceInDb(false);

            using (var _db = new DatabaseContext())
            {
                //Operation is carried out without errors
                var response = _os.deleteService(service1.Id, _db);
                Assert.AreEqual(1, response);
                //Operation does not take affect until save changes
                Assert.AreEqual(1, _os.deleteService(service1.Id, _db));
                _db.SaveChanges();
                //Service could not be found
                Assert.AreEqual(-1, _os.deleteService(service1.Id, _db));
            }
        }
    }
}
