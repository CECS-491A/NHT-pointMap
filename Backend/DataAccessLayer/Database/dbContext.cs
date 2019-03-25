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
        const string LOCAL_SQL_SERVER = "AV-XPS";
        const string LOCAL_DB_NAME = "NightWatchDB";

        string username = "Administrator";
        string password = "";
        string hostname = "pointmapdbinstance.cqugps36mcx5.us-east-2.rds.amazonaws.com";
        string port = "1433";
        string dbname = "pointmapdbinstance";

        public DatabaseContext()
        {
            //dev
            //this.Database.Connection.ConnectionString = string.Format(
            //    "Data Source={0};Initial Catalog={1};Integrated Security=True",
            //    LOCAL_SQL_SERVER, LOCAL_DB_NAME
            //    );

            //release
            this.Database.Connection.ConnectionString = "Data Source=" + hostname + ";Initial Catalog=" + dbname + ";User ID=" + username + ";Password=" + password + ";";
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Point> Points { get; set; }
    }
}
