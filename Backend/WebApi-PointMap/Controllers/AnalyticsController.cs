using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace WebApi_PointMap.Controllers
{
    public class AnalyticsController : ApiController
    {
        [HttpGet]
        [Route("api/analytics/usage")]
        public HttpResponseMessage UsageAnalytics()
        {
            var _analyticsService = new AnalyticsService();
            var analyticsData = _analyticsService.getAnalyticsData();
            return analyticsData;
        }
    }
}
