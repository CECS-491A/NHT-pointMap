using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

namespace UnitTesting
{
    [TestClass]
    public class ClaimServiceUT
    {
        ClaimService claimService;
        User user1;
        User user2;
        Service service1;
        Service service2;
        Claim claim1;
        public ClaimServiceUT()
        {
            var testUtils = new TestingUtils();

            claimService = new ClaimService();

            user1 = testUtils.createUser();
            user2 = testUtils.createUser();

            service1 = testUtils.createService(true);
            service2 = testUtils.createService(true);

            claim1 = testUtils.createClaim(user1, service1);
        }

        [TestMethod]
        public void getService()
        {
            Service received = claimService.getService(service1.ServiceName);

            StringAssert.Contains(received.ServiceName, service1.ServiceName);
        }

        [TestMethod]
        public void addServiceToUser()
        {
            claimService.addServiceToUser(user2, service2);

            using (var _db = new DatabaseContext())
            {
                int count = _db.Claims
                    .Where(c => c.UserId == user2.Id && c.ServiceId == service2.Id)
                    .Count();
                
                Assert.IsTrue(count > 0);
            }
        }

        [TestMethod]
        public void userHasServiceAccess()
        {
            bool hasAccess = claimService.userHasServiceAccess(user1, service1);

            Assert.IsTrue(hasAccess);
        }
    }
}
