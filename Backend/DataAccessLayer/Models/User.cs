using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class User
    {
        public User()
        {
            this.Sessions = new HashSet<Session>();
        }

        public long Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedAt { get; set; }

        public ICollection<Session> Sessions { get; set; }
    }
}
