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
using System.Web.Script.Serialization;
using System.Security.Cryptography;

namespace ServiceLayer.Services
{
    public class LoggingService : ILoggingService
    {
        //private const string LOG_SERVER_URL = "https://julianjp.com/logging/";
        private const string LOG_SERVER_URL = "http://localhost:3000/log";
        public static string LOGGER_API_SECRET = "CHRISTOPHER-123456-NIGHTWATCH-POINTMAP";

        public string GenerateSignature(string plaintext)
        {
            HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(LOGGER_API_SECRET));
            byte[] SignatureBuffer = Encoding.ASCII.GetBytes(plaintext);
            byte[] signatureBytes = hmacsha1.ComputeHash(SignatureBuffer);
            return Convert.ToBase64String(signatureBytes);
        }

        public System.Net.HttpStatusCode sendLogSync(LogRequestDTO newLog)
        {
            try
            {
                using (var client = new HttpClient())
                { 
                    var result = client.PostAsync(LOG_SERVER_URL, getLogContent(newLog)).Result;
                    int attempt = 1;
                    while (!result.IsSuccessStatusCode)
                    {
                        result = client.PostAsync(LOG_SERVER_URL, getLogContent(newLog)).Result;
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

        public async Task<System.Net.HttpStatusCode> sendLogAsync(LogRequestDTO newLog)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.PostAsync(LOG_SERVER_URL, getLogContent(newLog)).ConfigureAwait(false);
                    int attempt = 1;
                    while (!result.IsSuccessStatusCode)
                    {
                        result = await client.PostAsync(LOG_SERVER_URL, getLogContent(newLog)).ConfigureAwait(false);
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

        public StringContent getLogContent(LogRequestDTO newLog)
        {
            
            var jsonContent = new JavaScriptSerializer().Serialize(newLog);
            var content = new StringContent(jsonContent.ToString(), Encoding.UTF8, "application/json");

            return content;
        }

        public bool notifyAdmin(System.Net.HttpStatusCode status, StringContent content)
        {
            if (status != System.Net.HttpStatusCode.OK)
            {
                var fromAddress = new MailAddress("nightwatch491@gmail.com", "Night Watch");
                var toAddress = new MailAddress("nightwatch491@gmail.com", "Night Watch");
                const string password = "NightWatch_123";
                const string subject = "System Admin Logging Notification";
                string body = "PointMap Logging Notification: \n\nStatus Code:\n" + status +"\n\nLog Content:\n" + content.ToString();

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
