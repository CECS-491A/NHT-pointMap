using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceLayer.Interfaces;
using System.Threading.Tasks;
using System.Net.Http;
using DTO;

namespace ServiceLayer.Services
{
    public class LoggingService : ILoggingService
    {
        private const string LOG_SERVER_URL = "https://julianjp.com/logging/";

        public System.Net.HttpStatusCode sendLogSync(LogRequestDTO newLog, string signature, string timestamp)
        {
            try
            {
                using (var client = new HttpClient())
                { 
                    var result = client.PostAsync(LOG_SERVER_URL, getLogContent(newLog, signature, timestamp)).Result;
                    int attempt = 1;
                    while (!result.IsSuccessStatusCode)
                    {
                        result = client.PostAsync(LOG_SERVER_URL, getLogContent(newLog, signature, timestamp)).Result;
                        attempt++;
                        if (attempt >= 100)
                        {
                            return result.StatusCode;
                        }
                    }
                    return result.StatusCode;
                }
            }
            catch (System.AggregateException ex)
            {
                return System.Net.HttpStatusCode.InternalServerError;
            }
        }

        public async Task<System.Net.HttpStatusCode> sendLogAsync(LogRequestDTO newLog, string signature, string timestamp)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.PostAsync(LOG_SERVER_URL, getLogContent(newLog, signature, timestamp)).ConfigureAwait(false);
                    int attempt = 1;
                    while (!result.IsSuccessStatusCode)
                    {
                        result = client.PostAsync(LOG_SERVER_URL, getLogContent(newLog, signature, timestamp)).Result;
                        attempt++;
                        if (attempt >= 100)
                        {
                            return result.StatusCode;
                        }
                    }
                    return result.StatusCode;
                }
            }
            catch (System.AggregateException ex)
            {
                return System.Net.HttpStatusCode.InternalServerError;
            }
        }

        private FormUrlEncodedContent getLogContent(LogRequestDTO newLog, string signature, string timestamp)
        {
            var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>( "ssoUserId", newLog.ssoUserId ),
                new KeyValuePair<string, string>( "email", newLog.email ),
                new KeyValuePair<string, string>( "timestamp", timestamp ),
                new KeyValuePair<string, string>( "signature", signature ),
                new KeyValuePair<string, string>( "source", newLog.source),
                new KeyValuePair<string, string>( "user", newLog.user ),
                new KeyValuePair<string, string>( "desc", newLog.desc ),
                new KeyValuePair<string, string>( "details", newLog.details ),
                new KeyValuePair<string, string>( "createdDate", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
            });

            return content;
        }
    }
}
