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
            Sessions = new HashSet<Session>();
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string SecurityQ1 { get; set; }
        public string SecurityQ1Answer { get; set; }
        public string SecurityQ2 { get; set; }
        public string SecurityQ2Answer { get; set; }
        public string SecurityQ3 { get; set; }
        public string SecurityQ3Answer { get; set; }

        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        public ICollection<Session> Sessions { get; set; }
    }
}
