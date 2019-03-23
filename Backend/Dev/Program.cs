using ManagerLayer;
using ManagerLayer.UserManagement;
using System;
using ManagerLayer.Logging;
using System.Threading.Tasks;
using ManagerLayer.Models;
using System.Net.Http;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {

            // create user
            // the business req's to crete a user should go in the manger
            //  - call services to create user and add to the database

            //            UserManagementManager umm = new UserManagementManager();
            //          Guid guid = new Guid("f8d0c634-159e-4e8a-a561-19bc118a1b49");
            //        UserManager userManger = new UserManager();
            //      umm.DeleteUser(guid);
           
            Console.ReadKey();
        }
    }
}
