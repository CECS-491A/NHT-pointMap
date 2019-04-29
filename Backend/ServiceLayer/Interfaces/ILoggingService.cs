using System.Threading.Tasks;
using DTO.DTOBase;
using System.Net.Http;

namespace ServiceLayer.Interfaces
{
    interface ILoggingService
    {
        System.Net.HttpStatusCode sendLogSync(BaseLogDTO newLog);
        Task<System.Net.HttpStatusCode> sendLogAsync(BaseLogDTO newLog);
        string GenerateSignature(string plaintext);
        string GetSalt();
        StringContent getLogContent(BaseLogDTO newLog);
        bool notifyAdmin(System.Net.HttpStatusCode status, StringContent content);
    }
}
