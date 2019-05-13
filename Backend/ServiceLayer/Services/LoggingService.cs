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
    /// <summary>
    /// LoggingService is in charge of sending requests to the Log Server
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private const string LOG_POST_URL = "https://julianjp.com/logging/log";
        private const string ERROR_POST_URL = "https://julianjp.com/logging/error";

        private readonly string LOGGER_API_SECRET = "CHRISTOPHER-123456-NIGHTWATCH-POINTMAP";

        /// <summary>
        /// Generates a signature to authenticate logging request using SHA256 and HMAC
        /// </summary>
        /// <param name="plaintext">The plaintext given to the hashing function</param>
        /// <returns>Returns a hash in base64</returns>
        public string GenerateSignature(string plaintext)
        {
            var hmacsha256 = new HMACSHA256(Encoding.ASCII.GetBytes(LOGGER_API_SECRET));
            byte[] SignatureBuffer = Encoding.ASCII.GetBytes(plaintext);//ASCII or Hex for NIST standard
            byte[] signatureBytes = hmacsha256.ComputeHash(SignatureBuffer);

            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// Generates a salt for randomizaiton of requests
        /// </summary>
        /// <returns>Returns a base64 random salt</returns>
        public string GetSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            var salt = new byte[32]; //Complies with NIST standard of at least 32 bits in length
            rng.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Attempts to send a request to logging server synchronously
        /// </summary>
        /// <param name="newLog">A BaseLogDTO object that will be sent</param>
        /// <returns>Returns the status code of the request</returns>
        public System.Net.HttpStatusCode sendLogSync(BaseLogDTO newLog) //Sends a log synchronously
        {
            string tempURL = getURL(newLog);
            try
            {
                using (var client = new HttpClient())
                { 
                    var result = client.PostAsync(tempURL, getLogContent(newLog)).Result; //.Result make it synchronous
                    int attempt = 1;
                    while (!result.IsSuccessStatusCode) //Will resend if request fails up to 100 times
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
            catch (Exception)
            {
                return System.Net.HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Attempts to send a request to logging server asynchronously
        /// </summary>
        /// <param name="newLog">A BaseLogDTO object that will be sent</param>
        /// <returns>Returns the status code of the request</returns>
        public async Task<System.Net.HttpStatusCode> sendLogAsync(BaseLogDTO newLog)
        {
            string tempURL = getURL(newLog);
            try
            {
                using (var client = new HttpClient())
                {
                    //Must await to check if it should attempt to retry request if the current request failed
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

        /// <summary>
        /// Takes the content inside the BaseLogDTO derived object and converts to json
        /// </summary>
        /// <param name="newLog">A BaseLogDTO object that will be converted</param>
        /// <returns>A StringContent containing the stringified json</returns>
        public StringContent getLogContent(BaseLogDTO newLog)
        {
            
            var jsonContent = new JavaScriptSerializer().Serialize(newLog);
            var content = new StringContent(jsonContent.ToString(), Encoding.UTF8, "application/json");

            return content;
        }

        /// <summary>
        /// Notifies the System Administrator via email if the request did not return a 200 response
        /// </summary>
        /// <param name="status">The http status code of the request</param>
        /// <param name="content">The content of the last request</param>
        /// <returns>A boolean that is true if mail was sent or status is 200</returns>
        public bool notifyAdmin(System.Net.HttpStatusCode status, StringContent content)
        {
            if (status != System.Net.HttpStatusCode.OK)
            {
                var fromAddress = new MailAddress("nightwatch491@gmail.com", "Night Watch");
                var toAddress = new MailAddress("nightwatch491@gmail.com", "Night Watch");
                const string password = "NightWatch_123";
                const string subject = "System Admin Logging Notification";
                string body = "PointMap Logging Notification: \n\nStatus Code:\n" + status +"\n\nLog Content:\n" + content.ToString();

                SmtpClient smtp = new SmtpClient //Initalizes mailclient
                {
                    Host = "smtp.gmail.com", //Gmail mail client host
                    Port = 587, //Port that gmail accepts smtp through
                    EnableSsl = true, //Request must be secured or will be rejected by gmail smtp server
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, password)
                };
                using(var message = new MailMessage(fromAddress, toAddress) //Creates message
                {
                    Subject = subject,
                    Body = body
                })
                try
                {
                    smtp.Send(message); //Attempts to send message
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

        /// <summary>
        /// Helper function which returns the correct url based on the object type of newLog
        /// </summary>
        /// <param name="newLog">A BaseLogDTO derived object</param>
        /// <returns>The URL which the HTTP client should send a POST request to</returns>
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
