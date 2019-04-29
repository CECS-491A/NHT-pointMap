using System;
using System.Threading.Tasks;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;

namespace Logging.Logging
{
    public class Logger
    {
        private LoggingService _ls;

        public Logger()
        {
            _ls = new LoggingService();
        }

        public bool sendLogSync(LogRequestDTO newLog)
        {
            string[] content = getContent(newLog); //Returns [signature, timestamp]
            newLog.signature = content[0];
            newLog.timestamp = content[1];
            if (newLog.isValid())
            {
                var responseStatusCode = _ls.sendLogSync(newLog);
                _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog));
            }
            return newLog.isValid();
        }

        public async Task<bool> sendLogAsync(LogRequestDTO newLog)
        {
            string[] content = getContent(newLog); //Returns [signature, timestamp]
            newLog.signature = content[0];
            newLog.timestamp = content[1];
            if (newLog.isValid())
            {
                var responseStatusCode = await _ls.sendLogAsync(newLog);
                _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog));
            }
            return newLog.isValid();
        }

        private string[] getContent(LogRequestDTO newLog)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string plaintext = "ssoUserId=" + newLog.ssoUserId + ";email=" + newLog.email + ";timestamp=" + timestamp + ";";
            string signature = _ls.GenerateSignature(plaintext);
            return new string[] { signature, timestamp };
        }

        public LogRequestDTO initalizeAnalyticsLog(string details, DTO.Constants.Constants.Sources source, User user = null, 
            Session session = null, DTO.Constants.Constants.Pages page = DTO.Constants.Constants.Pages.None)
        {
            LogRequestDTO newLog = new LogRequestDTO();
            newLog.source = source;
            newLog.details = details;
            newLog.success = true;
            if(page != DTO.Constants.Constants.Pages.None)
                newLog.page = page;
            if(session != null)
            {
                newLog.sessionCreatedAt = session.CreatedAt;
                newLog.sessionExpiredAt = session.ExpiresAt;
                newLog.sessionUpdatedAt = session.UpdatedAt;
                newLog.token = session.Token;
            }
            if(user != null)
            {
                newLog.ssoUserId = user.Id.ToString();
                newLog.email = user.Username;
            }

            return newLog;
        }

        public bool sendErrorLog(DTO.Constants.Constants.Sources source, string details, string id = null, string email = null,
            DTO.Constants.Constants.Pages page = DTO.Constants.Constants.Pages.None, Session session = null)
        {
            LogRequestDTO newLog = new LogRequestDTO();
            newLog.source = source;
            newLog.success = false;
            newLog.details = details;
            if (id == null)
                newLog.ssoUserId = "Invalid User";
            else
                newLog.ssoUserId = id;
            if (email == null)
                newLog.email = "Invalid Email";
            else
                newLog.email = email;
            if (page != null)
                newLog.page = page;
            if(session != null)
            {
                newLog.sessionCreatedAt = session.CreatedAt;
                newLog.sessionExpiredAt = session.ExpiresAt;
                newLog.sessionUpdatedAt = session.UpdatedAt;
                newLog.token = session.Token;
            }

            return sendLogSync(newLog);
        }
    }
}
