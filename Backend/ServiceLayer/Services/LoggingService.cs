using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceLayer.Interfaces;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Mail;
using System.Net;
using DTO;
using System.Security.Cryptography;

namespace ServiceLayer.Services
{
    public class LoggingService : ILoggingService
    {
        private const string LOG_SERVER_URL = "https://julianjp.com/logging/";
        public static string LOGGER_API_SECRET = "CHRISTOPHER-123456-NIGHTWATCH-POINTMAP";

        public class RequestPayloadAuthentication
        {
            public string GenerateSignature(string plaintext)
            {
                HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(LOGGER_API_SECRET));
                byte[] SignatureBuffer = Encoding.ASCII.GetBytes(plaintext);
                byte[] signatureBytes = hmacsha1.ComputeHash(SignatureBuffer);
                return Convert.ToBase64String(signatureBytes);
            }
        }

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
            catch (System.AggregateException)
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
                        result = await client.PostAsync(LOG_SERVER_URL, getLogContent(newLog, signature, timestamp)).ConfigureAwait(false);
                        attempt++;
                        if (attempt >= 100)
                        {
                            return result.StatusCode;
                        }
                    }
                    return result.StatusCode;
                }
            }
            catch (System.AggregateException)
            {
                return System.Net.HttpStatusCode.InternalServerError;
            }
        }

        public FormUrlEncodedContent getLogContent(LogRequestDTO newLog, string signature, string timestamp)
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

        public bool notifyAdmin(System.Net.HttpStatusCode notify, FormUrlEncodedContent content)
        {
            if (notify != System.Net.HttpStatusCode.OK)
            {
                var fromAddress = new MailAddress("nightwatch491@gmail.com", "Night Watch");
                var toAddress = new MailAddress("nightwatch491@gmail.com", "Night Watch");
                const string password = "NightWatch_123";
                const string subject = "System Admin Logging Notification";
                string body = "PointMap Logging Notification: \n\nStatus Code:\n" + notify +"\n\nLog Content:\n" + System.Text.Encoding.Default.GetString(content.ReadAsByteArrayAsync().Result);

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, password)
                };
                using(var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                try
                {
                    smtp.Send(message);
                    return true;
                }catch(Exception e)
                {
                    Console.WriteLine("Error notifying system admin due to exception {0}",
                        e.ToString());
                    return false;
                }
            }
            return true;
        }
    }
}
