using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DTO.DTO
{
    public class ErrorRequestDTO
    {
        [Required]
        public DateTime logCreatedAt { get; set; }

        [Required]
        public string ssoUserId { get; set; }

        [Required]
        public string details { get; set; }

        public Constants.Constants.Sources source { get; set; }

        [Required]
        public string signature { get; set; }
    }
}
