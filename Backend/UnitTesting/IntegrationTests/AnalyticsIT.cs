using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTesting;
using ServiceLayer.Services;
using System.Net.Http;

namespace Testing.IntegrationTests
{
    [TestClass]
    public class AnalyticsIT
    {
        TestingUtils _tu;
        AnalyticsService _as;
        
        public AnalyticsIT()
        {
            _tu = new TestingUtils();
            _as = new AnalyticsService();
        }

        [TestMethod]
        public void getAnalyticsData200()
        {
            HttpResponseMessage response = _as.getAnalyticsData();
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void SendTestAnalytics()
        {
            _tu.SendAnalytics();
        }

        [TestMethod]
        public void SendTestErrors()
        {
            _tu.SendErrors();
        }
    }
}
