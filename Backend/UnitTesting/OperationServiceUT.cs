using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using DataAccessLayer.Database;

namespace UnitTesting
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
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
                Assert.AreEqual(1, _os.createService(service1, _db));
                Assert.AreEqual(0, _os.createService(service2, _db));
            }
        }

        [TestMethod]
        public void createServiceString()
        {
            string serviceName = "" + Guid.NewGuid();
            using (var _db = new DatabaseContext())
            {
                Assert.AreEqual(1, _os.createService(serviceName, _db));
            }
        }

        [TestMethod]
        public void disableService()
        {
            service1 = _ts.CreateService(true);
            service2 = _ts.CreateService(true);

            using (var _db = new DatabaseContext())
            {
                Assert.AreEqual(1, _os.disableService(service1.Id, _db));
                Assert.AreEqual(1, _os.disableService(service2.Id, _db));
            }
        }

        [TestMethod]
        public void enableService()
        {
            service1 = _ts.CreateService(false);
            service2 = _ts.CreateService(false);

            using (var _db = new DatabaseContext())
            {
                Assert.AreEqual(1, _os.enableService(service1.Id, _db));
                Assert.AreEqual(1, _os.enableService(service2.Id, _db));
            }
        }

        [TestMethod]
        public void deleteService()
        {
            service1 = _ts.CreateService(false);

            using (var _db = new DatabaseContext())
            {
                Assert.AreEqual(1, _os.deleteService(service1.Id, _db));
                Assert.AreEqual(1, _os.deleteService(service1.Id, _db));
                _db.SaveChanges();
                Assert.AreEqual(0, _os.deleteService(service1.Id, _db));
            }
        }
    }
}
