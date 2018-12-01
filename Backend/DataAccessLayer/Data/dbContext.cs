using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace DataAccessLayer.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }


    }
}
