using System;
using System.Threading.Tasks;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;
using DTO.DTOBase;

namespace Logging.Logging
{
    public class Logger
    {
        private LoggingService _ls;

        public Logger()
        {
            _ls = new LoggingService();
        }

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

        public async Task<bool> sendLogAsync(BaseLogDTO newLog)
        {
            newLog = getContent(newLog);
            if (newLog.isValid())
            {
                var responseStatusCode = await _ls.sendLogAsync(newLog);
                _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog));
            }
            return newLog.isValid();
        }

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

        public ErrorRequestDTO initalizeErrorLog(string details, DTO.Constants.Constants.Sources source)
        {
            ErrorRequestDTO newError = new ErrorRequestDTO(details, source);

            return newError;
        }
    }
}
