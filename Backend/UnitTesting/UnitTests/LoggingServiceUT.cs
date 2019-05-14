using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using UnitTesting;

namespace Testing.UnitTests
{
    [TestClass]
    public class LoggingServiceUT
    {
        LoggingService _ls;
        TestingUtils _tu;
        LogRequestDTO newLog;
        ErrorRequestDTO newError;

        User newUser;
        Session newSession;
        public LoggingServiceUT()
        {
            _ls = new LoggingService();
            _tu = new TestingUtils();

            newUser = _tu.CreateUserObject();
            newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);
        }

        [TestMethod]
        public void notifySystemAdmin()
        {
            newLog = new LogRequestDTO();
            newLog.setSource(DTO.Constants.Constants.Sources.Login);
            newLog.details = "testing stacktrace";
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            newLog = (LogRequestDTO)_tu.getLogContent(newLog);
            var content = _ls.getLogContent(newLog);
            bool adminNotified = _ls.notifyAdmin(System.Net.HttpStatusCode.Unauthorized, content);
            Assert.IsTrue(adminNotified);
        }

        [TestMethod]
        public void dontNotifySystemAdmin()
        {
            newLog = new LogRequestDTO();
            newLog.setSource(DTO.Constants.Constants.Sources.AdminDashboard);
            newLog.details = "testing stacktrace";
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            newLog = (LogRequestDTO)_tu.getLogContent(newLog); //[signature, timestamp]
            var content = _ls.getLogContent(newLog);
            bool adminNotified = _ls.notifyAdmin(System.Net.HttpStatusCode.OK, content);
            Assert.IsTrue(adminNotified);
        }

        [TestMethod]
        public void ErrorSyncResponse200()
        {
            newError = new ErrorRequestDTO();
            newError.setSource(DTO.Constants.Constants.Sources.AdminDashboard);
            newError.details = "This is a test error";
            newError = (ErrorRequestDTO)_tu.getLogContent(newError);
            var responseStatus = _ls.sendLogSync(newError);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void ErrorSyncResponse400()
        {
            newError = new ErrorRequestDTO();
            newError.setSource(DTO.Constants.Constants.Sources.AdminDashboard);
            newError = (ErrorRequestDTO)_tu.getLogContent(newError);
            var responseStatus = _ls.sendLogSync(newError);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ErrorSyncResponse401()
        {
            newError = new ErrorRequestDTO();
            newError.setSource(DTO.Constants.Constants.Sources.AdminDashboard);
            newError.details = "This is a test error";
            var responseStatus = _ls.sendLogSync(newError);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void LogSyncResponse200()
        {
            newLog = new LogRequestDTO();
            newLog.setSource(DTO.Constants.Constants.Sources.AdminDashboard);
            newLog.details = "testing stacktrace";
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            newLog = (LogRequestDTO)_tu.getLogContent(newLog);
            var responseStatus = _ls.sendLogSync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void LogSyncResponse400()
        {
            newLog = new LogRequestDTO(); //Missing Required Source field
            newLog.details = "testing stacktrace";
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            newLog = (LogRequestDTO)_tu.getLogContent(newLog); //[signature, timestamp]
            var responseStatus = _ls.sendLogSync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void LogSyncResponse401()
        {
            newLog = new LogRequestDTO();
            newLog.setSource(DTO.Constants.Constants.Sources.AdminDashboard);
            newLog.details = "testing stacktrace";
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
            newLog.setSource(DTO.Constants.Constants.Sources.AdminDashboard);
            newLog.details = "testing stacktrace";
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            newLog = (LogRequestDTO)_tu.getLogContent(newLog); //[signature, timestamp]
            var responseStatus = await _ls.sendLogAsync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.OK);

        }

        [TestMethod]
        public async Task LogAsyncResponse400()
        {
            newLog = new LogRequestDTO(); //Missing Required Source field
            newLog.details = "testing stacktrace";
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.sessionCreatedAt = newSession.CreatedAt;
            newLog.sessionExpiredAt = newSession.ExpiresAt;
            newLog.sessionUpdatedAt = newSession.UpdatedAt;
            newLog.token = newSession.Token;
            newLog = (LogRequestDTO)_tu.getLogContent(newLog); //[signature, timestamp]
            var responseStatus = await _ls.sendLogAsync(newLog);
            Assert.AreEqual(responseStatus, System.Net.HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public async Task LogAsyncResponse401()
        {
            newLog = new LogRequestDTO(); //Missing signature, timestamp and salt
            newLog.details = "testing stacktrace";
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
