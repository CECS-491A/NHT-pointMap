using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace WebApi_PointMap.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}