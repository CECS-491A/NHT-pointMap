using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ManagerLayer.Models
{
    public class LogRequestDTO
    {
        [Required]
        public string ssoUserId { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string timestamp { get; set; }
        [Required]
        public string signature { get; set; }

        [Required]
        public string source { get; set; }
        [Required]
        public string user { get; set; }
        [Required]
        public string desc { get; set; }
    }
}
