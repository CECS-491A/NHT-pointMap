using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Session
    {
        public Session()
        {
            CreateAt = DateTime.UtcNow;
        }

        [Required]
        public string Token { get; set; }
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        public DateTime ExpiresAt { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt { get; set; }

        [Required]
        [ForeignKey("User")]
        [Column("UserInSession")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
