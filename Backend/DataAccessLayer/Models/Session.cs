using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Session
    {
        public string Token { get; set; }
        public long Id { get; set; }
        public string ExpiresAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CreateAt { get; set; }

        public User UserInSession { get; set; }
    }
}
