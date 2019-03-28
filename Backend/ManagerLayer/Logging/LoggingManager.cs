using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using DTO;
using ServiceLayer.Services;

namespace ManagerLayer.Logging
{
    public class LoggingManager
    {
        private const string LOG_SERVER_URL = "http://localhost:3000";
        private readonly HttpClient client;
        private TokenService _ts;
        private LoggingService _ls;

        public LoggingManager()
        {
            _ts = new TokenService();
            _ls = new LoggingService();
            client = new HttpClient();
        }

        public void sendLogSync(LogRequestDTO newLog)
        {
            string[] content = getContent(newLog);
            var responseStatusCode = _ls.sendLogSync(newLog, content[0],
                content[1]);
            notifyAdmin(responseStatusCode);
        }

        public async Task sendLogAsync(LogRequestDTO newLog)
        {
            string[] content = getContent(newLog);
            var responseStatusCode = await _ls.sendLogAsync(newLog, content[0],
                content[1]);
            notifyAdmin(responseStatusCode);
        }

        private void notifyAdmin(System.Net.HttpStatusCode notify)
        {
            if (notify != System.Net.HttpStatusCode.OK)
            {
                //TODO if notify is true notify system admin of failed log
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
