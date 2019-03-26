using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagerLayer.Models;
using ManagerLayer.Logging;
using System.Threading.Tasks;
using System.Net.Http;

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
        public void request200Log()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";
            var content = LoggingManager.getLogContent(newLog);

            HttpResponseMessage response = LoggingManager.sendLogAsync(content);
            Console.WriteLine(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void request401Log()
        {
            newLog = new LogRequestDTO();
            newLog.email = "asd";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.user = "test123";
            newLog.desc = "Testing description";

            var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>( "ssoUserId", newLog.ssoUserId ),
                new KeyValuePair<string, string>( "email", newLog.email ),
                new KeyValuePair<string, string>( "timestamp", "123" ),
                new KeyValuePair<string, string>( "signature", "1235234" ),
                new KeyValuePair<string, string>( "source", newLog.source),
                new KeyValuePair<string, string>( "user", newLog.user ),
                new KeyValuePair<string, string>( "desc", newLog.desc ),
                new KeyValuePair<string, string>( "createdDate", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
            });

            HttpResponseMessage response = LoggingManager.sendLogAsync(content);
            Console.WriteLine(response);
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void request400Log()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "";
            newLog.user = "";
            newLog.desc = "Testing description";
            var content = LoggingManager.getLogContent(newLog);

            HttpResponseMessage response = LoggingManager.sendLogAsync(content);
            Console.WriteLine(response);
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }
    }
}
