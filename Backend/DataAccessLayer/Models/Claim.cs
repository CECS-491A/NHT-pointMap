using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Claim
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey("Service")]
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
