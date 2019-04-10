using System.ComponentModel.DataAnnotations;

namespace DTO.DTO
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
}
