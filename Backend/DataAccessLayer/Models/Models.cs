using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataAccess.Models
{
    public class UserContext : DbContext
    {
        public UserContext (DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }

    public class User
    {
        public User()
        {
            this.sessions = new HashSet<Session>();
        }

        public long id { get; set; }
        public string userName { get; set; }
        public string passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }

        public ICollection<Session> sessions { get; set; }
    }

    public class Session
    {
        public string Token { get; set; }
        public long Id { get; set; }
        public string ExperiationOn { get; set; }
        public string UpdatedAt { get; set; }
        public string CreateAt { get; set; }

        public User UserInSession { get; set; }
    }
}
