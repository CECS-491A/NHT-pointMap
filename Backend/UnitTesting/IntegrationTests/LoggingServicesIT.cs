﻿using UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Logging.Logging;

namespace Testing.IntegrationTests
{
    [TestClass]
    public class LoggingServicesIT
    {
        LoggingService _ls;
        TestingUtils _tu;
        Logger logger;
        ErrorRequestDTO newError;
        LogRequestDTO newLog;
        

        User newUser;
        Session newSession;

        public LoggingServicesIT()
        {
            _tu = new TestingUtils();
            logger = new Logger();

            newUser = _tu.CreateUserObject();
            newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);
        }

        [TestMethod]
        public void validLogRequestDTO()
        {
            newLog = new LogRequestDTO(DTO.Constants.Constants.Sources.AdminDashboard, newUser.Id.ToString(), newSession.CreatedAt,
                newSession.ExpiresAt, newSession.UpdatedAt, newSession.Token);
            newLog = (LogRequestDTO)_tu.getLogContent(newLog); //Valid after initalization and retrieving auth contents

            Assert.IsTrue(newLog.isValid());
        }

        [TestMethod]
        public void invalidLogRequestDTO()
        {
            newLog = new LogRequestDTO();
            newLog.details = "Test details";
            newLog.setSource(DTO.Constants.Constants.Sources.Session);
            newLog = (LogRequestDTO)_tu.getLogContent(newLog);

            Assert.IsFalse(newLog.isValid()); //No userId
        }

        [TestMethod]
        public void sendErrorSyncPass()
        {
            newError = new ErrorRequestDTO("This is a new error log", DTO.Constants.Constants.Sources.AdminDashboard);
            Assert.IsTrue(logger.sendLogSync(newError));
        }

        [TestMethod]
        public async Task sendErrorAsyncPass()
        {
            newError = new ErrorRequestDTO("This is a new error log", DTO.Constants.Constants.Sources.AdminDashboard);
            Assert.IsTrue(await logger.sendLogAsync(newError));
        }

        [TestMethod]
        public void sendErrorSyncFail()
        {
            newError = new ErrorRequestDTO();
            Assert.IsFalse(logger.sendLogSync(newError));
        }

        [TestMethod]
        public async Task sendErrorAsyncFail()
        {
            newError = new ErrorRequestDTO();
            Assert.IsFalse(await logger.sendLogAsync(newError));
        }

        [TestMethod]
        public void sendLogSyncPass()
        {
            newLog = new LogRequestDTO(DTO.Constants.Constants.Sources.AdminDashboard, newUser.Id.ToString(), newSession.CreatedAt,
                newSession.ExpiresAt, newSession.UpdatedAt, newSession.Token);
            Assert.IsTrue(logger.sendLogSync(newLog));
        }

        [TestMethod]
        public void sendLogSyncFail() 
        {
            newLog = new LogRequestDTO();
            newLog.setSource(DTO.Constants.Constants.Sources.Logout);
            Assert.IsFalse(logger.sendLogSync(newLog));//Missing userId

            newLog = new LogRequestDTO();
            newLog.ssoUserId = newUser.Id.ToString();
            Assert.IsFalse(logger.sendLogSync(newLog)); //Missing source
        }

        [TestMethod]
        public async Task sendLogAsyncPass()
        {
            newLog = new LogRequestDTO(DTO.Constants.Constants.Sources.AdminDashboard, newUser.Id.ToString(), newSession.CreatedAt,
                newSession.ExpiresAt, newSession.UpdatedAt, newSession.Token);
            Assert.IsTrue(await logger.sendLogAsync(newLog));
        }

        [TestMethod]
        public async Task sendLogAsyncFail()
        {
            newLog = new LogRequestDTO();
            newLog.setSource(DTO.Constants.Constants.Sources.Logout);
            Assert.IsFalse(logger.sendLogSync(newLog));
            Assert.IsFalse(await logger.sendLogAsync(newLog)); //Missing UserId


            newLog = new LogRequestDTO();
            newLog.ssoUserId = newUser.Id.ToString();
            Assert.IsFalse(logger.sendLogSync(newLog));
            Assert.IsFalse(await logger.sendLogAsync(newLog)); //Missing Source
        }
    }
}
