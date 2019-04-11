using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace DataAccessLayer.Database
{
    public class DatabaseContext : DbContext
    {
        // TODO: turn into enviornmental variables for dev and deploy

        public DatabaseContext()
        {
            var connectionString = Environment.GetEnvironmentVariable("NW_POINTMAP_DEV_DATABASE", EnvironmentVariableTarget.User);
            this.Database.Connection.ConnectionString = connectionString;
        }

        public void RevertDatabaseChanges(DatabaseContext _db)
        {
            foreach (var entry in _db.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Point> Points { get; set; }
    }
}
