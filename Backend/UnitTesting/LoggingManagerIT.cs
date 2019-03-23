using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagerLayer.Models;
using ManagerLayer.Logging;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestClass]
    public class LoggingManagerIT
    {
        LogRequestDTO newLog;
        LoggingManager _lm;

        public LoggingManagerIT()
        {
            _lm = new LoggingManager();
        }

        

        [TestMethod]
        public void successfulLog()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.signature = "4T5Csu2U9OozqN66Us+pEc5ODcBwPs1ldaq2fmBqtfo=";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.timestamp = "1552766624957";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";

            System.Net.Http.HttpResponseMessage response = LoggingManager.sendLogAsync(newLog);
            Console.WriteLine(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void unSuccessfulLog()
        {
            newLog = new LogRequestDTO();
            newLog.email = "asd";
            newLog.signature = "4T5Csu2U9OozqN66Us+pEc5ODcBwPs1ldaq2fmBqtfo=";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.timestamp = "1552766624957";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";

            System.Net.Http.HttpResponseMessage response = LoggingManager.sendLogAsync(newLog);
            Console.WriteLine(response);
            Assert.IsTrue(!response.IsSuccessStatusCode);
        }
    }
}
