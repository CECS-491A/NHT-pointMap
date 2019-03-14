using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.Models
{
    public class LoginManagerDTOs
    {
        
    }

    public class LoginManagerResponseDTO
    {
        public Guid userid { get; set; }
        public string token { get; set; }
    }
}
