using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerLayer.Models;
using System.Net.Http;

namespace ManagerLayer.Logging
{
    public class LoggingManager
    {
        private const string LOG_SERVER_URL = "http://localhost:3000";
        private static readonly HttpClient client = new HttpClient();

        public static HttpResponseMessage sendLogAsync(LogRequestDTO newLog)
        {

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>( "ssoUserId", newLog.ssoUserId ),
                new KeyValuePair<string, string>( "email", newLog.email ),
                new KeyValuePair<string, string>( "timestamp", newLog.timestamp ),
                new KeyValuePair<string, string>( "signature", newLog.signature ),
                new KeyValuePair<string, string>( "source", newLog.source),
                new KeyValuePair<string, string>( "user", newLog.user ),
                new KeyValuePair<string, string>( "desc", newLog.desc ),
                new KeyValuePair<string, string>( "createdDate", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
            });

                var result = client.PostAsync(LOG_SERVER_URL, content).Result;
                //string resultContent = await result.Content.ReadAsStringAsync();
                
                return result;
            }
        }
    }
}
