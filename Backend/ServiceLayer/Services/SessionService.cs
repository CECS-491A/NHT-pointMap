using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class SessionService : ISessionService
    {

        public string GenerateSession()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 /2];
            provider.GetBytes(b);
            string hex = BitConverter.ToString(b).Replace("-","");
            return hex;
        }
 
    }
}
