using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Session
    {
        public long Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ExpiresAt { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt { get; set; }

        // foreign key
        public int UserId { get; set; }
        // Nav property
        public virtual User UserInSession { get; set; }
    }
}
