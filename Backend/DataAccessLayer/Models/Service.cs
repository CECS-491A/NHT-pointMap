using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Service
    {
        public Service()
        {
            Disable = false;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ServiceName { get; set; }
        public bool Disable { get; set; }
    }
}
