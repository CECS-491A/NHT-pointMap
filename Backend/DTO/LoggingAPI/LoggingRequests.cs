using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.LoggingAPI
{
    public class LogWebpageUsageRequest
    {

        [Required]
        public string Page { get; set; }

        [Required]
        public long DurationStart { get; set; }

        [Required]
        public long DurationEnd { get; set; }

        // Passed in as a Request Header [token]
        public string Token { get; set; }

        // TODO: Make required
        public string Signature { get; set; }

        // TODO: Make Required
        public string Salt { get; set; }

        public long GetDuration()
        {
            return DurationEnd - DurationStart;
        }
    }

}
