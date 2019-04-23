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
        private const string LOG_SERVER_URL = "https://julianjp.com/logging/";
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
            _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog, content[0], content[1]));
        }

        public async Task sendLogAsync(LogRequestDTO newLog)
        {
            string[] content = getContent(newLog);
            var responseStatusCode = await _ls.sendLogAsync(newLog, content[0],
                content[1]);
            _ls.notifyAdmin(responseStatusCode, _ls.getLogContent(newLog, content[0], content[1]));
        }

        private string[] getContent(LogRequestDTO newLog)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string plaintext = "ssoUserId=" + newLog.ssoUserId + ";email=" + newLog.email +
                ";timestamp=" + timestamp + ";";
            var _logServiceAuth = new LoggingService.RequestPayloadAuthentication();
            string signature = _logServiceAuth.GenerateSignature(plaintext);
            return new string[] { signature, timestamp };
        }
    }
}
