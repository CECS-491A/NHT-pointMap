using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.KFCSSO_API
{
    public class SSO_API_Requests
    {
    }

    public class LoginRequestPayload
    {
        [Required]
        public string SSOUserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public long Timestamp { get; set; }
        [Required]
        public string Signature { get; set; }
    }

    public class DeleteUserFromSSO_DTO
    {
        [Required]
        public string AppId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Signature { get; set; }
        [Required]
        public string SsoUserId { get; set; }
        [Required]
        public long Timestamp { get; set; }
    }
}
