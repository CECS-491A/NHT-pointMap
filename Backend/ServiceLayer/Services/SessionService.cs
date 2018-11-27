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
    }
}
