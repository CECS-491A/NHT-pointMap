using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DTO.KFCSSO_API
{
    class SSO_API_Responses
    {
    }

    public class SSOLoginResponse
    {
        public static HttpResponseMessage ResponseRedirect(ApiController controller, string redirectURL)
        {
            var redirect = controller.Request.CreateResponse(HttpStatusCode.SeeOther);
            redirect.Headers.Location = new Uri(redirectURL);
            return redirect;
        }
    }
}
