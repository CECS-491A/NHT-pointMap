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

        [Required, ForeignKey("OwnerUser"), Column(Order = 1)]
        public Guid UserId { get; set; }
        public virtual User OwnerUser { get; set; }

        [ForeignKey("SubjectUser"), Column(Order = 2)]
        public Guid? SubjectUserId { get; set; }
        public virtual User SubjectUser { get; set; }

        [Required, ForeignKey("Service"), Column(Order = 3)]
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("Client"), Column(Order = 4)]
        public Guid? ClientId { get; set; }
        public virtual Client Client { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
