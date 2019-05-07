using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.UserManagementAPI
{
    public class UpdateUserRequestDTO
    {
        [Required]
        public string Id { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Manager { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public bool Disabled { get; set; }
    }

    public class CreateUserRequestDTO
    {
        [Required]
        public string Username { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Manager { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public bool Disabled { get; set; }
    }
}
