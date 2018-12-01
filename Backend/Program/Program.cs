using DataAccessLayer.Data;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var _db = new DatabaseContext())
            {
                //var u = new User {
                //    Email = "alfredo@mail.com",
                //};
                //_db.Users.Add(u);
                //_db.SaveChanges();
                var users = _db.Users;

                foreach (var user in users.ToList<User>())
                {
                    Console.WriteLine(user.Email);
                }
                Console.ReadKey();
            }
        }
    }
}
