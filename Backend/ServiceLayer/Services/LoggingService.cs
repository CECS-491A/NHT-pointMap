using System;
using System.Text;
using ServiceLayer.Interfaces;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Mail;
using System.Net;
using DTO;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using DTO.DTOBase;

namespace ServiceLayer.Services
{
    public class LoggingService : ILoggingService
    {
        //private const string LOG_POST_URL = "https://julianjp.com/logging/log";
        private const string LOG_POST_URL = "http://localhost:3000/log";
        private const string ERROR_POST_URL = "http://localhost:3000/error";
        public static string LOGGER_API_SECRET = "CHRISTOPHER-123456-NIGHTWATCH-POINTMAP";

        public string GenerateSignature(string plaintext)
        {
            HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(LOGGER_API_SECRET));
            byte[] SignatureBuffer = Encoding.ASCII.GetBytes(plaintext);//ASCII or Hex for NIST standard
            byte[] signatureBytes = hmacsha1.ComputeHash(SignatureBuffer);
            return Convert.ToBase64String(signatureBytes);
        }

        public string GetSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            var salt = new byte[32]; //Complies with NIST standard of at least 32 bits in length
            rng.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        public System.Net.HttpStatusCode sendLogSync(BaseLogDTO newLog) //Sends a log synchronously
        {
            string tempURL = getURL(newLog);
            try
            {
                using (var client = new HttpClient())
                { 
                    var result = client.PostAsync(tempURL, getLogContent(newLog)).Result; //.Result make it sync
                    int attempt = 1;
                    while (!result.IsSuccessStatusCode)
                    {
                        //Must reinitalize logContent everytime a post attempt is made 
                        result = client.PostAsync(tempURL, getLogContent(newLog)).Result; 
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

        public async Task<System.Net.HttpStatusCode> sendLogAsync(BaseLogDTO newLog)
        {
            string tempURL = getURL(newLog);
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.PostAsync(tempURL, getLogContent(newLog)).ConfigureAwait(false);
                    int attempt = 1;
                    while (!result.IsSuccessStatusCode)
                    {
                        result = await client.PostAsync(tempURL, getLogContent(newLog)).ConfigureAwait(false);
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

        public StringContent getLogContent(BaseLogDTO newLog) //Turn the log request DTO into a JSON string
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

        private string getURL(BaseLogDTO newLog)
        {
            string tempURL;
            if (newLog.GetType().Equals(typeof(LogRequestDTO)))
            {
                tempURL = LOG_POST_URL;
            }
            else
            {
                tempURL = ERROR_POST_URL;
            }
            return tempURL;
        }
    }
}
