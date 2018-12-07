using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ClientRepository
    {
        public int CreateClient(Client client)
        {
            client.UpdatedAt = DateTime.UtcNow;
            using (var _db = new DatabaseContext())
            {
                _db.Clients.Add(client);
                return _db.SaveChanges();
            }
        }
    }
}
