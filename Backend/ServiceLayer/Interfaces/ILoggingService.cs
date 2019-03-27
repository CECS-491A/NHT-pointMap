using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using DTO;

namespace ServiceLayer.Interfaces
{
    interface ILoggingService
    {
        System.Net.HttpStatusCode sendLogSync(LogRequestDTO newLog, string signature, string timestamp);
        Task<System.Net.HttpStatusCode> sendLogAsync(LogRequestDTO newLog, string signature, string timestamp);
    }
}
