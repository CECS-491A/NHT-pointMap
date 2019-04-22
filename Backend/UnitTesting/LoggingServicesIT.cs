using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using System.Net.Http;  

namespace UnitTesting
{
    [TestClass]
    public class LoggingServicesIT
    {
        LogRequestDTO newLog;
        LoggingService _ls;
        TestingUtils _tu;

        public LoggingServicesIT()
        {
            _ls = new LoggingService();
            _tu = new TestingUtils();
        }
        
        [TestMethod]
        public void validLogRequestDTO()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.details = "testing stacktrace";
            var content = _tu.getLogContent(newLog); //signature timestamp
            newLog.signature = content[0];
            newLog.timestamp = content[1];

            Assert.IsTrue(newLog.isValid());
        }

        [TestMethod]
        public void invalidLogRequestDTO()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";

            Assert.IsFalse(newLog.isValid());
        }

        //[TestMethod]
        //public void notifySystemAdminPass()
        //{
        //    newLog = new LogRequestDTO();
        //    newLog.email = "julianpoyo+22@gmail.com";
        //    newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
        //    newLog.source = "testingClass";
        //    newLog.details = "testing stacktrace";
        //    var content = _ls.getLogContent(newLog);
        //    bool adminNotified = _ls.notifyAdmin(System.Net.HttpStatusCode.Unauthorized, content);
        //    Assert.IsTrue(adminNotified);
        //}

        [TestMethod]
        public void LogSyncResponse200()
        {
            newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = "testingClass";
            newLog.details = "testing stacktrace";
            var content = _tu.getLogContent(newLog); //signature timestamp
            newLog.signature = content[0];
            newLog.timestamp = content[1];
            var responseStatus = _ls.sendLogSync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void createSessions()
        {
            newLog = new LogRequestDTO();
            newLog.source = "testingClass";
            newLog.details = "testing stacktrace";
            Random rand = new Random();
            for(var i = 0; i < 20; i++)
            {
                User newUser = _tu.CreateUserObject();
                Session newSession = _tu.CreateSessionObject(newUser);
                _tu.CreateSessionInDb(newSession);
                newLog.email = newUser.Username;
                newLog.ssoUserId = newUser.Id.ToString();
                newLog.logCreatedAt = new DateTime(2018, 11, 21);
                for (var j = 0; j < 3; j++)
                {
                    newLog.success = true;
                    newLog.page = LogRequestDTO.loginPage;
                    if (j == 0)
                        newLog.page = LogRequestDTO.registrationPage;
                    var duration = rand.Next(1, 1000);
                    if(duration < 200)
                        newLog.success = false;
                    newLog.sessionCreatedAt = newSession.CreatedAt;
                    newLog.sessionExpiredAt = newSession.ExpiresAt.AddSeconds(duration);
                    newLog.sessionUpdatedAt = newSession.UpdatedAt.AddSeconds(duration);
                    newLog.token = newSession.Token;
                    var content = _tu.getLogContent(newLog); //signature timestamp
                    newLog.signature = content[0];
                    newLog.timestamp = content[1];
                    var responseStatus = _ls.sendLogSync(newLog);
                    Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
                }
            }
            
        }

        //[TestMethod]
        //public async Task LogAsyncResponse200()
        //{
        //    newLog = new LogRequestDTO();
        //    newLog.email = "julianpoyo+22@gmail.com";
        //    newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
        //    newLog.source = "testingClass";
        //    var responseStatus = await _ls.sendLogAsync(newLog);
        //    Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
        //}

        //[TestMethod]
        //public void LogSyncResponse401()
        //{
        //    newLog = new LogRequestDTO();
        //    newLog.email = "julianpoyo+22@gmail.com";
        //    newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
        //    newLog.source = "testingClass";
        //    newLog.details = "testing stacktrace";

        //    var responseStatus = _ls.sendLogSync(newLog);
        //    Console.WriteLine(responseStatus);
        //    Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.Unauthorized);

        //}

        //[TestMethod]
        //public async Task LogAsyncResponse401()
        //{
        //    newLog = new LogRequestDTO();
        //    newLog.email = "julianpoyo+22@gmail.com";
        //    newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
        //    newLog.source = "testingClass";
        //    newLog.details = "testing stacktrace";
        //    var responseStatus = await _ls.sendLogAsync(newLog);

        //    Console.WriteLine(responseStatus);
        //    Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.Unauthorized);
        //}

        //[TestMethod]
        //public void LogSyncResponse400()
        //{
        //    newLog = new LogRequestDTO();
        //    newLog.email = "julianpoyo+22@gmail.com";
        //    newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
        //    newLog.source = "testingClass";
        //    newLog.details = "testing stacktrace";
        //    var responseStatus = _ls.sendLogSync(newLog);

        //    Console.WriteLine(responseStatus);
        //    Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);
        //}

        //[TestMethod]
        //public async Task LogAsyncResponse400()
        //{
        //    newLog = new LogRequestDTO();
        //    newLog.email = "julianpoyo+22@gmail.com";
        //    newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
        //    newLog.source = "testingClass";
        //    newLog.details = "testing stacktrace";
        //    var responseStatus = await _ls.sendLogAsync(newLog);

        //    Console.WriteLine(responseStatus);
        //    Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);
        //}
    }
}
