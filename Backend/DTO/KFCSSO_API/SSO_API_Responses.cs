using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DTO.KFCSSO_API
{
    class SSO_API_Responses
    {
    }

    public class SSOLoginResponse
    {
        public static HttpResponseMessage ResponseRedirect(HttpRequestMessage request, string redirectURL)
        {
            var redirect = request.CreateResponse(HttpStatusCode.SeeOther);
            redirect.Headers.Location = new Uri(redirectURL);
            return redirect;
        }
    }
}
