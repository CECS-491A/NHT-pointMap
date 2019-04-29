using System.Threading.Tasks;
using DTO.DTOBase;

namespace ServiceLayer.Interfaces
{
    interface ILoggingService
    {
        System.Net.HttpStatusCode sendLogSync(BaseLogDTO newLog);
        Task<System.Net.HttpStatusCode> sendLogAsync(BaseLogDTO newLog);
    }
}
