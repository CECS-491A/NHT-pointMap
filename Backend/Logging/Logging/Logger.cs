using System;
using System.Threading.Tasks;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using DTO.DTOBase;

namespace Logging.Logging
{
    /// <summary>
    /// Logger is a class in charge of initalizing and validating BaseLogDTO derived objects
    /// </summary>
    public class Logger
    {
        private LoggingService _ls;

        /// <summary>
        /// Base constructor for logger
        /// </summary>
        public Logger()
        {
            _ls = new LoggingService();
        }

        /// <summary>
        /// Sends a derivative object of BaseLogDTO to the logging server synchronously
        /// </summary>
        /// <param name="newLog">A derivative of the BaseLogDTO object</param>
        /// <returns>A boolean whether the object passed was a valid object</returns>
        public bool sendLogSync(BaseLogDTO newLog)
        {
            newLog = getContent(newLog);
            if (newLog.isValid())
            {
                var responseStatusCode = _ls.sendLogSync(newLog);
                _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog));
            }
            return newLog.isValid();
        }

        /// <summary>
        /// Sends a derivative object of BaseLogDTO to the logging server asynchronously
        /// </summary>
        /// <param name="newLog">A derivative of the BaseLogDTO object</param>
        /// <returns>A boolean whether the object passed was a valid object</returns>
        public async Task<bool> sendLogAsync(BaseLogDTO newLog)
        {
            newLog = getContent(newLog);
            if (newLog.isValid()) //Only send if the object it validated
            {
                var responseStatusCode = await _ls.sendLogAsync(newLog);
                _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog));
            }
            return newLog.isValid();
        }

        /// <summary>
        /// Retrieves and appends authentication content required for all derivatives of BaseLogDTO objects
        /// </summary>
        /// <param name="newLog">A derivative of the BaseLogDTO object</param>
        /// <returns>The appended BaseLogDTO object</returns>
        private BaseLogDTO getContent(BaseLogDTO newLog)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string salt = _ls.GetSalt();
            string plaintext = "timestamp=" + timestamp + ";salt=" + salt;
            string signature = _ls.GenerateSignature(plaintext);
            newLog.timestamp = timestamp;
            newLog.salt = salt;
            newLog.signature = signature;
            return newLog;
        }

        /// <summary>
        /// Initalizes and fills a LogRequestDTO object
        /// </summary>
        /// <param name="source">A DTO.Constants.Constants.Sources enum value</param>
        /// <param name="userId">A string of a userId</param>
        /// <param name="session">A DataAccessLayer.Models.Session object</param>
        /// <returns>An initalized LogRequestDTO object</returns>
        public LogRequestDTO initalizeAnalyticsLog(DTO.Constants.Constants.Sources source, string userId, Session session = null)
        {
            LogRequestDTO newLog = new LogRequestDTO(userId, source);
            if(session != null)
            {
                newLog.sessionCreatedAt = session.CreatedAt;
                newLog.sessionExpiredAt = session.ExpiresAt;
                newLog.sessionUpdatedAt = session.UpdatedAt;
                newLog.token = session.Token;
            }

            return newLog;
        }

        /// <summary>
        /// Initalizes and fills an ErrorRequestDTO object
        /// </summary>
        /// <param name="details">A string stack trace of the error</param>
        /// <param name="source">A DTO.Constants.Constants.Sources enum value</param>
        /// <returns>An initalized ErrorRequestDTO object</returns>
        public ErrorRequestDTO initalizeErrorLog(string details, DTO.Constants.Constants.Sources source)
        {
            ErrorRequestDTO newError = new ErrorRequestDTO(details, source);

            return newError;
        }
    }
}
