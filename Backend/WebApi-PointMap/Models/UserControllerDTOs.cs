using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi_PointMap.Models
{
    public class LoginDTO
    {
        [Required]
        public string SSOUserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public long Timestamp { get; set; }
        [Required]
        public string Signature { get; set; }
        
        public string PreSignatureString()
        {
            string acc = "";
            acc += "ssoUserId=" + SSOUserId + ";";
            acc += "email=" + Email + ";";
            acc += "timestamp=" + Timestamp + ";";
            return acc;
        }
    }

    public class LoginResponseDTO
    {
        public string RedirectURI { get; set; }
    }
}