using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class SessionService : ISessionService
    {
        public string getFoo()
        {
            return "foo";
        }

        public Guid generateSession()
        {
            IPasswordService ip = new PasswordService();
            string str = System.Text.Encoding.Default.GetString(ip.GenerateSalt());
            Guid guid = new Guid(str);
            Console.WriteLine("New Guid is " + guid);
            Console.ReadLine();
            return guid;
        }
    }
}
