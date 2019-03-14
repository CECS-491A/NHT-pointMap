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
        public string Name { get; set; }
    }

    public class LoginResponseDTO
    {
        public string token { get; set; }
        public Guid userId { get; set; }
    }
}