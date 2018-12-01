using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string DateOfBirth { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string UpdatedAt { get; set; }

        public string CreatedAt { get; set; }

    }
}
