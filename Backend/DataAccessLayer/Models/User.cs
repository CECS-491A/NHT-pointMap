using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class User
    {
        public User()
        {
            CreatedAt = DateTime.UtcNow;
            Disabled = false;
        }

        [Key]
        public Guid Id{ get; set; }

        [Required]
        public string Username { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [ForeignKey("Manager")]
        public Guid? ManagerId { get; set; }
        public User Manager { get; set; }

        [Required]
        public bool Disabled { get; set; }

        [Required, Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Column(TypeName = "datetime2"), DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
