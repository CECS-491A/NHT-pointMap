using DTO;
using ServiceLayer.Services;
using System.Web.Http.ExceptionHandling;

namespace WebApi_PointMap.ErrorHandling
{
    public class GlobalErrorLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            //TODO : how to retrieve other info
            LoggingService _ls = new LoggingService();
            LogRequestDTO newLog = new LogRequestDTO();
            newLog.email = "julianpoyo+22@gmail.com";
            newLog.ssoUserId = "0743cd2c-fec3-4b79-a5b6-a6c52a752c71";
            newLog.source = context.Exception.Source;
            newLog.details = context.Exception.StackTrace;
            //TODO : signature, check timestamp
            //var responseStatus = _ls.sendLogSync(newLog, "4T5Csu2U9OozqN66Us+pEc5ODcBwPs1ldaq2fmBqtfo=",
            //    DateTime.UtcNow.ToString());

            base.Log(context);
        }
    }
}