using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;
using System.Net.Http;

namespace UnitTesting
{
    [TestClass]
    public class AnalyticsServiceIT
    {
        AnalyticsService _as;
        TestingUtils _tu;

        public AnalyticsServiceIT()
        {
            _tu = new TestingUtils();
            _as = new AnalyticsService();
        }
        [TestMethod]
        public void response200()
        {
            HttpResponseMessage response = _as.getAnalyticsData();
            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(content);
            Assert.IsNotNull(response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void addLogs()
        {
            _tu.createLogs();
        }
    }
}
