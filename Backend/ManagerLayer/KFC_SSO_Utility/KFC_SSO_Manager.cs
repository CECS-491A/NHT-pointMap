using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceLayer.Services.ExceptionService;

namespace ManagerLayer.KFC_SSO_Utility
{
    public class KFC_SSO_Manager
    {
        public async Task<bool> DeleteUserFromSSOviaPointmap(User user)
        {
            var _ssoAPI = new KFC_SSO_APIService();
            var requestResponse = await _ssoAPI.DeleteUserFromSSO(user);
            if (requestResponse.IsSuccessStatusCode)
            {
                return true;
            }
            // throw response of request
            throw new KFCSSOAPIRequestException(requestResponse.Content.ToString());
        }
    }
}
