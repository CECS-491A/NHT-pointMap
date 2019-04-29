using DataAccessLayer.Models;
using DTO.KFCSSO_API;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ServiceLayer.KFC_API_Services
{
    public class SSO_APIService
    {
        const string API_URL = "https://api.kfc-sso.com";
        const string APP_ID = "69CA4919-0B26-40DA-AF1E-775FFADED87C";
        public static readonly string APISecret = "5E5DDBD9B984E4C95BBFF621DF91ABC9A5318DAEC0A3B231B4C1BC8FE0851610";

        public async Task<HttpResponseMessage> DeleteUserFromSSO(User user)
        {
            var auth = new SignatureService();
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var payloadToSign = auth.PreparePayload(user.Id.ToString(), user.Username, timestamp);
            var signature = auth.Sign(payloadToSign);
            var requestPayload = new DeleteUserFromSSO_DTO
            {
                AppId = APP_ID,
                Email = user.Username,
                SsoUserId = user.Id.ToString(),
                Timestamp = timestamp,
                Signature = signature
            };
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
