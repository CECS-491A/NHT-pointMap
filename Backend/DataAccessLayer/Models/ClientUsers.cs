using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class ClientUsers
    {
        public ClientUsers()
        {
            CreatedAt = DateTime.UtcNow;
            ClientId = Guid.NewGuid();
        }

        [Key, Column(Order = 0), ForeignKey("Client")]
        public Guid ClientId { get; set; }
        public Client Client { get; set; }

        [Key, Column(Order = 1), ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
