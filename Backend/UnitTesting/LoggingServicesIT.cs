using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Logging.Logging;

namespace UnitTesting
{
    [TestClass]
    public class LoggingServicesIT
    {
        LogRequestDTO newLog;
        LoggingService _ls;
        TestingUtils _tu;
        Logger logger;

        User newUser;
        Session newSession;

        public LoggingServicesIT()
        {
            _tu = new TestingUtils();
            logger = new Logger();
            newLog = new LogRequestDTO();

            newUser = _tu.CreateUserObject();
            newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);
        }

        [TestMethod]
        public void validLogRequestDTO()
        {
            

            newLog = logger.initalizeAnalyticsLog("Testing", newLog.adminDashSource, newUser, newSession);
            var content = _tu.getLogContent(newLog); //signature timestamp
            newLog.signature = content[0];
            newLog.timestamp = content[1];

            Assert.IsTrue(newLog.isValid());

            newLog = logger.initalizeAnalyticsLog("Testing", newLog.adminDashSource, newUser);
            content = _tu.getLogContent(newLog); //signature timestamp
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

            newLog.details = "Test details";
            newLog.ssoUserId = newUser.Id.ToString();
            newLog.email = newUser.Username;
            newLog.source = "Invalid Source";
            var content = _tu.getLogContent(newLog); //signature timestamp
            newLog.signature = content[0];
            newLog.timestamp = content[1];

            Assert.IsFalse(newLog.isValid());
        }

        [TestMethod]
        public void sendValidErrorLog()
        {
            Assert.IsTrue(logger.sendErrorLog(newLog.sessionSource, "deatils", null, null, null));
            Assert.IsTrue(logger.sendErrorLog(newLog.sessionSource, "deatils", newUser.Id.ToString(), newUser.Username, null));
        }

        [TestMethod]
        public void sendInalidErrorLog()
        {
            Assert.IsFalse(logger.sendErrorLog("Invalid source", "deatils", null, null, null));
            Assert.IsFalse(logger.sendErrorLog("Invalid source", "deatils", newUser.Id.ToString(), newUser.Username, null));
        }

        [TestMethod]
        public void sendLogSyncPass()
        {
            newLog = logger.initalizeAnalyticsLog("Testing Log", newLog.adminDashSource, newUser, newSession, newLog.adminDashPage);
            Assert.IsTrue(logger.sendLogSync(newLog));

            newLog = logger.initalizeAnalyticsLog("Testing Log", newLog.adminDashSource, newUser);
            Assert.IsTrue(logger.sendLogSync(newLog));
        }

        [TestMethod]
        public void sendLogSyncFail()
        {
            newLog = logger.initalizeAnalyticsLog("Testing Log", "Invalid source", newUser, newSession, newLog.adminDashPage);
            Assert.IsFalse(logger.sendLogSync(newLog));

            newLog = logger.initalizeAnalyticsLog("Testing Log", newLog.adminDashSource, newUser, null, "Invalid page");
            Assert.IsFalse(logger.sendLogSync(newLog));
        }

        [TestMethod]
        public async Task sendLogAsyncPass()
        {
            newLog = logger.initalizeAnalyticsLog("Testing Log", newLog.adminDashSource, newUser, newSession, 
                newLog.adminDashPage);
            Assert.IsTrue(await logger.sendLogAsync(newLog));

            newLog = logger.initalizeAnalyticsLog("Testing Log", newLog.adminDashSource, newUser);
            Assert.IsTrue(await logger.sendLogAsync(newLog));
        }

        [TestMethod]
        public async Task sendLogAsyncFail()
        {
            newLog = logger.initalizeAnalyticsLog("Testing Log", "Invalid source", newUser, newSession, 
                newLog.adminDashPage);
            Assert.IsFalse(await logger.sendLogAsync(newLog));

            newLog = logger.initalizeAnalyticsLog("Testing Log", newLog.adminDashSource, newUser, null, "Invalid page");
            Assert.IsFalse(await logger.sendLogAsync(newLog));
        }
    }
}
