using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Claim
    {
        public Claim()
        {
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required, ForeignKey("User"), Column(Order = 1)]
        public Guid UserId { get; set; }
        public Guid UserId2 { get; set; }
        public User User { get; set; }

        //[ForeignKey("User"), Column(Order = 2)]
        //public Guid UserId2 { get; set; }
        //public User User2 { get; set; }

        [Required, ForeignKey("Service")]
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("Client")]
        public Guid ClientId { get; set; }
        public Client Client { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
