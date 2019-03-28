using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using ServiceLayer.Services;
using System.Threading.Tasks;
using System.Net.Http;  

namespace UnitTesting
{
    [TestClass]
    public class LoggingServicesIT
    {
        LogRequestDTO newLog;
        LoggingService _ls;

        public LoggingServicesIT()
        {
            _ls = new LoggingService();
        }



        [TestMethod]
        public void LogSyncResponse200()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";
            newLog.details = "testing stacktrace";
            var responseStatus = _ls.sendLogSync(newLog, "4T5Csu2U9OozqN66Us+pEc5ODcBwPs1ldaq2fmBqtfo=",
                "1552766624957");
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
        }



        [TestMethod]
        public async Task LogAsyncResponse200()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";
            newLog.details = "testing stacktrace";
            var responseStatus = await _ls.sendLogAsync(newLog, "4T5Csu2U9OozqN66Us+pEc5ODcBwPs1ldaq2fmBqtfo=",
                "1552766624957");
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void LogSyncResponse401()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";
            newLog.details = "testing stacktrace";

            var responseStatus = _ls.sendLogSync(newLog, "abscu",
            "1552766624957");
            Console.WriteLine(responseStatus);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.Unauthorized);
            
        }

        [TestMethod]
        public async Task LogAsyncResponse401()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";
            newLog.details = "testing stacktrace";
            var responseStatus = await _ls.sendLogAsync(newLog, "abscu",
                "1552766624957");

            Console.WriteLine(responseStatus);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void LogSyncResponse400()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "";
            newLog.details = "testing stacktrace";
            var responseStatus = _ls.sendLogSync(newLog, "4T5Csu2U9OozqN66Us+pEc5ODcBwPs1ldaq2fmBqtfo=",
                "1552766624957");

            Console.WriteLine(responseStatus);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task LogAsyncResponse400()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "";
            newLog.details = "testing stacktrace";
            var responseStatus = await _ls.sendLogAsync(newLog, "4T5Csu2U9OozqN66Us+pEc5ODcBwPs1ldaq2fmBqtfo=",
                "1552766624957");

            Console.WriteLine(responseStatus);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);
        }
    }
}
