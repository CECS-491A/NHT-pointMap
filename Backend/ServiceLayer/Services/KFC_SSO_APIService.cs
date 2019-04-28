using DataAccessLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static DTO.DTO.SSOServicesDTOs;
using static ServiceLayer.Services.ExceptionService;

namespace ServiceLayer.Services
{
    public class KFC_SSO_APIService
    {
        const string API_URL = "https://api.kfc-sso.com";
        const string APP_ID = "69CA4919-0B26-40DA-AF1E-775FFADED87C";
        public static readonly string APISecret = "5E5DDBD9B984E4C95BBFF621DF91ABC9A5318DAEC0A3B231B4C1BC8FE0851610";

        // Request authentication to/from SSO API, signing and validation utility service
        public class RequestPayloadAuthentication
        {
            public bool IsValidClientRequest(Guid userId, string email, long timestamp, string signature)
            {
                // Dictionary represents the signed body of the request to the destination server
                // Props can be added to this, and they will be added to signature
                var payload = PreparePayload(userId, email, timestamp);
                var generatedSignature = Sign(payload);
                return generatedSignature == signature;
            }

            public Dictionary<string, string> PreparePayload(Guid userId, string email, long timestamp)
            {
                var preparedPayload = new Dictionary<string, string>();
                preparedPayload.Add("ssoUserId", userId.ToString());
                preparedPayload.Add("email", email);
                preparedPayload.Add("timestamp", timestamp.ToString());
                return preparedPayload;
            }

            // Signs a dictionary with the provided key by constructing a key/value string
            public string Sign(Dictionary<string, string> payload)
            {
                // Order the provided dictionary by key
                // This is necessary so that the recipient of the payload will be able to generate the
                // correct hash even if the order changes
                var orderedPayload = from payloadItem in payload
                                     orderby payloadItem.Value descending
                                     select payloadItem;

                var payloadString = "";
                // Build a payload string with the format:
                // key =value;key2=value2;
                // SECURITY: This must be passed in this format so that the resulting hash is the same
                foreach (var pair in orderedPayload)
                {
                    payloadString = payloadString + pair.Key + "=" + pair.Value + ";";
                }

                var signature = Sign(payloadString);
                return signature;
            }

            // Signs a string with the provided key
            public string Sign(string payloadString)
            {
                // Instantiate a new hashing algorithm with the provided key
                HMACSHA256 hashingAlg = new HMACSHA256(Encoding.ASCII.GetBytes(APISecret));

                // Get the raw bytes from our payload string
                byte[] payloadBuffer = Encoding.ASCII.GetBytes(payloadString);

                // Calculate our hash from the byte array
                byte[] signatureBytes = hashingAlg.ComputeHash(payloadBuffer);

                var signature = Convert.ToBase64String(signatureBytes);
                return signature;
            }
        }

        public async Task<HttpResponseMessage> DeleteUserFromSSO(User user)
        {
            var requestPayload = new DeleteUserFromSSO_DTO
            {
                AppId = APP_ID,
                Email = user.Username,
                SsoUserId = user.Id.ToString(),
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            };
            var _tokenService = new TokenService();
            var auth = new RequestPayloadAuthentication();
            requestPayload.Signature = auth.Sign(requestPayload.PreSignatureString());
            var response = await PingDeleteUserFromSSORouteAsync(requestPayload);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            throw new KFCSSOAPIRequestException("Delete User from SSO request error.");
        }

        public async Task<HttpResponseMessage> PingDeleteUserFromSSORouteAsync(DeleteUserFromSSO_DTO payload)
        {
            HttpClient client = new HttpClient();
            var stringPayload = JsonConvert.SerializeObject(payload);
            var jsonPayload = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(API_URL + "/api/users/appdeleteuser", jsonPayload);
            return request;
        }
    }
}
