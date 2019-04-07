using ManagerLayer;
using ManagerLayer.UserManagement;
using System;
using ManagerLayer.Logging;
using System.Threading.Tasks;
using System.Net.Http;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.GetEnvironmentVariable("NW_POINTMAP_DEV_DATABASE", EnvironmentVariableTarget.User));
            Console.ReadKey();
        }
    }
}
