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
        public static readonly string APISecret = "FDF9B8E2935D6F4C7336604164B92B82E36D1BD87FF96333194D41FDDA023449";

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
            return GenerateSignature(presignuatureString) == signature;
        }

        public string GenerateSignature(string plaintext)
        {
            HMACSHA256 hmacsha1 = new HMACSHA256(Encoding.ASCII.GetBytes(APISecret));
            byte[] SignatureBuffer = Encoding.ASCII.GetBytes(plaintext);
            byte[] signatureBytes = hmacsha1.ComputeHash(SignatureBuffer);
            return Convert.ToBase64String(signatureBytes);
        }

    }
}
