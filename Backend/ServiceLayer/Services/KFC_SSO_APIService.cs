using DataAccessLayer.Models;
using DTO.KFCSSO_API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ServiceLayer.Services
{
    public class KFC_SSO_APIService
    {
        const string API_URL = "https://api.kfc-sso.com";
        const string APP_ID = "69CA4919-0B26-40DA-AF1E-775FFADED87C";
        public static readonly string APISecret = "5E5DDBD9B984E4C95BBFF621DF91ABC9A5318DAEC0A3B231B4C1BC8FE0851610";

        public class RequestPayloadAuthentication
        {
            public bool IsValidClientRequest(string presignuatureString, string signature)
            {
                var generatedSignature = GenerateSignature(presignuatureString);
                return generatedSignature == signature;
            }

            public string GenerateSignature(string plaintext)
            {
                HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(APISecret));
                byte[] SignatureBuffer = Encoding.ASCII.GetBytes(plaintext);
                byte[] signatureBytes = hmacsha1.ComputeHash(SignatureBuffer);
                return Convert.ToBase64String(signatureBytes);
            }
        }

        public HttpResponseMessage DeleteUserFromSSO(User user)
        {
            var requestPayload = new DeleteUserFromSSO_DTO
            {
                AppId = APP_ID,
                Email = user.Username,
                SsoUserId = user.Id.ToString(),
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            };
            var auth = new RequestPayloadAuthentication();
            requestPayload.Signature = auth.GenerateSignature(requestPayload.PreSignatureString());
            var response = PingDeleteUserFromSSORouteAsync(requestPayload);
            if (response.IsCompleted)
            {
                return response.Result;
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
