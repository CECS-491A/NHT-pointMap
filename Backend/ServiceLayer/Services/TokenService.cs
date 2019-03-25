using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class TokenService
    {
        private readonly string APISecret = "E32EB6E8C4F301572A03F538996907E87D9397CEF3B28A8D520D17210713E0C3";

        public string GenerateToken()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 / 2];
            provider.GetBytes(b);
            string hex = BitConverter.ToString(b).Replace("-", "");
            return hex;
        }

        public bool isValidSignature(string presignuatureString, string signature)
        {
            HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(APISecret));
            // genereate signature using the payload information to get an attempted signature
            byte[] SignatureBuffer = Encoding.ASCII.GetBytes(presignuatureString);
            byte[] signatureBytes = hmacsha1.ComputeHash(SignatureBuffer);
            string resultSignature = Convert.ToBase64String(signatureBytes);
            return resultSignature == signature;
        }

    }
}
