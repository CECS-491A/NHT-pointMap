using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class User
    {
        public User()
        {
            this.Sessions = new HashSet<Session>();
        }

        public long Id { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Username is not an email.")]
        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
