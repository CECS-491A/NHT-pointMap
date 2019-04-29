using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DTO.DTOBase
{
    /// <summary>
    /// Base class for all LogDTO objects
    /// </summary>
    public abstract class BaseLogDTO
    {
        [Required]
        public DateTime logCreatedAt { get; set; }

        [Required]
        public string source { get; protected set; }

        [Required]
        public string timestamp { get; set; }

        [Required]
        public string signature { get; set; }

        [Required]
        public string salt { get; set; }

        public abstract bool isValid();
    }
}
