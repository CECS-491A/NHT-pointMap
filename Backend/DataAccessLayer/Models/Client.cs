using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Client
    {
        public Client()
        {
            CreatedAt = DateTime.UtcNow;
            ClientId = Guid.NewGuid();
        }


        [Key]
        public Guid ClientId { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public bool Disabled { get; set; }

        public string ClientAddress { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
