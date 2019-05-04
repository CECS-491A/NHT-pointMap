using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using WebApi_PointMap.ErrorHandling;
using WebApi_PointMap.Filters;

namespace WebApi_PointMap
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable CORS globaly across all controllers
            EnableCorsAttribute cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");

            // Web API configuration and services
            config.EnableCors(cors);


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
