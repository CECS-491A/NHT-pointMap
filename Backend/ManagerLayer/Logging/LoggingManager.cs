using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using DTO;
using ServiceLayer.Services;
using DataAccessLayer.Models;

namespace ManagerLayer.Logging
{
    public class LoggingManager
    {
        private const string LOG_SERVER_URL = "http://localhost:3000/";
        private readonly HttpClient client;
        private TokenService _ts;
        private LoggingService _ls;

        public LoggingManager()
        {
            _ts = new TokenService();
            _ls = new LoggingService();
            client = new HttpClient();
        }

        public LogRequestDTO addSessionToLog(LogRequestDTO newLog, Session session)
        {
            newLog.sessionCreatedAt = session.CreatedAt;
            newLog.sessionExpiredAt = session.ExpiresAt;
            newLog.sessionUpdatedAt = session.UpdatedAt;
            newLog.token = session.Token;
            newLog.ssoUserId = session.User.Id.ToString();
            newLog.email = session.User.Username;
            return newLog;
        }

        public void sendLogSync(LogRequestDTO newLog)
        {
            if(newLog.isValid())
            {
                string[] content = getContent(newLog); //Returns [signature, timestamp]
                newLog.signature = content[0];
                newLog.timestamp = content[1];
                var responseStatusCode = _ls.sendLogSync(newLog);
                _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog));
            }
            
        }

        public async Task sendLogAsync(LogRequestDTO newLog)
        {
            if(newLog.isValid())
            {
                string[] content = getContent(newLog); //Returns [signature, timestamp]
                newLog.signature = content[0];
                newLog.timestamp = content[1];
                var responseStatusCode = await _ls.sendLogAsync(newLog);
                _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog));
            }
        }

        private string[] getContent(LogRequestDTO newLog)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string plaintext = "ssoUserId=" + newLog.ssoUserId + ";email=" + newLog.email +
                ";timestamp=" + timestamp + ";";
            string signature = _ts.GenerateSignature(plaintext);
            return new string[] { signature, timestamp };
        }
    }
}
