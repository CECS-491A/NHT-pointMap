using ManagerLayer.UserManagers.Management;
using System;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {

            // create user
            // the business req's to crete a user should go in the manger
            //  - call services to create user and add to the database

            UserManagementManager umm = new UserManagementManager();
            umm.CreateUser("tester2@mail.com", "tester2password", DateTime.UtcNow);
            Console.ReadKey();
        }
    }
}
