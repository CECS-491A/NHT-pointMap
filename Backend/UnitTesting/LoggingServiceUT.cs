using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestClass]
    public class LoggingServiceUT
    {
        LogRequestDTO newLog;
        LoggingService _ls;
        TestingUtils _tu;

        User newUser;
        Session newSession;
        public LoggingServiceUT()
        {
            _ls = new LoggingService();
            _tu = new TestingUtils();
            newLog = new LogRequestDTO();

            newUser = _tu.CreateUserObject();
            newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);
        }

        [TestMethod]
        public void notifySystemAdmin()
        {
            newLog = new LogRequestDTO();
            newLog.source = newLog.adminDashSource;
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            var auth = _tu.getLogContent(newLog); //[signature, timestamp]
            newLog.signature = auth[0];
            newLog.timestamp = auth[1];
            var content = _ls.getLogContent(newLog);
            bool adminNotified = _ls.notifyAdmin(System.Net.HttpStatusCode.Unauthorized, content);
            Assert.IsTrue(adminNotified);
        }

        [TestMethod]
        public void dontNotifySystemAdmin()
        {
            newLog = new LogRequestDTO();
            newLog.source = newLog.adminDashSource;
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            var auth = _tu.getLogContent(newLog); //[signature, timestamp]
            newLog.signature = auth[0];
            newLog.timestamp = auth[1];
            var content = _ls.getLogContent(newLog);
            bool adminNotified = _ls.notifyAdmin(System.Net.HttpStatusCode.OK, content);
            Assert.IsTrue(adminNotified);
        }

        [TestMethod]
        public void LogSyncResponse200()
        {
            newLog = new LogRequestDTO();
            newLog.source = newLog.adminDashSource;
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            var content = _tu.getLogContent(newLog); //[signature, timestamp]
            newLog.signature = content[0];
            newLog.timestamp = content[1];
            var responseStatus = _ls.sendLogSync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void LogSyncResponse400()
        {
            newLog = new LogRequestDTO(); //Missing Required Source field
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            var content = _tu.getLogContent(newLog); //[signature, timestamp]
            newLog.signature = content[0];
            newLog.timestamp = content[1];
            var responseStatus = _ls.sendLogSync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void LogSyncResponse401()
        {
            newLog = new LogRequestDTO();
            newLog.source = newLog.adminDashSource;
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token; //Missing signature and timestamp
            var responseStatus = _ls.sendLogSync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.Unauthorized);

        }

        [TestMethod]
        public async Task LogAsyncResponse200()
        {
            newLog = new LogRequestDTO();
            newLog.source = newLog.adminDashSource;
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            var content = _tu.getLogContent(newLog); //[signature, timestamp]
            newLog.signature = content[0];
            newLog.timestamp = content[1];
            var responseStatus = await _ls.sendLogAsync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);

        }

        [TestMethod]
        public async Task LogAsyncResponse400()
        {
            newLog = new LogRequestDTO(); //Missing Required Source field
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            var content = _tu.getLogContent(newLog); //[signature, timestamp]
            newLog.signature = content[0];
            newLog.timestamp = content[1];
            var responseStatus = await _ls.sendLogAsync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public async Task LogAsyncResponse401()
        {
            newLog = new LogRequestDTO(); //Missing signature and timestamp
            newLog.details = "testing stacktrace";
            newLog.email = newUser.Username;
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            var responseStatus = await _ls.sendLogAsync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
