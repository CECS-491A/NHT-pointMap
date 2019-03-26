using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerLayer.Models;
using System.Net.Http;
using ServiceLayer.Services;

namespace ManagerLayer.Logging
{
    public class LoggingManager
    {
        private const string LOG_SERVER_URL = "http://localhost:3000";
        private static readonly HttpClient client = new HttpClient();
        private static TokenService _ts = new TokenService();

        public static HttpResponseMessage sendLogAsync(FormUrlEncodedContent content)
        {

            using (var client = new HttpClient())
            {
                var result = client.PostAsync(LOG_SERVER_URL, content).Result;
                //string resultContent = await result.Content.ReadAsStringAsync();
                
                return result;
            }
        }

        public static FormUrlEncodedContent getLogContent(LogRequestDTO newLog)
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string plaintext = "ssoUserId=" + newLog.ssoUserId + ";email=" + newLog.email +
                ";timestamp=" + timestamp + ";";

            string signature = _ts.GenerateSignature(plaintext);

            var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>( "ssoUserId", newLog.ssoUserId ),
                new KeyValuePair<string, string>( "email", newLog.email ),
                new KeyValuePair<string, string>( "timestamp", timestamp ),
                new KeyValuePair<string, string>( "signature", signature ),
                new KeyValuePair<string, string>( "source", newLog.source),
                new KeyValuePair<string, string>( "user", newLog.user ),
                new KeyValuePair<string, string>( "desc", newLog.desc ),
                new KeyValuePair<string, string>( "createdDate", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
            });

            return content;
        }
    }
}
