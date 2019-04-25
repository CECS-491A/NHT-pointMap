using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLayer.Interfaces;
using System.Net.Http;
using System.Net;
using System.Text;
using System.IO;

namespace ServiceLayer.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private const string ANALYTICS_SERVER_URL = "https://julianjp.com/logging/";

        public HttpResponseMessage getAnalyticsData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    int i = 0;
                    var result = client.GetAsync(ANALYTICS_SERVER_URL).Result;

                    while (!result.IsSuccessStatusCode || i >= 100)
                    {
                        i++;
                        result = client.GetAsync(ANALYTICS_SERVER_URL).Result;
                    }
                    return result;
                }
            }
            catch (System.AggregateException ex)
            {
                HttpResponseMessage serverError = new HttpResponseMessage();
                serverError.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                serverError.Content = new StringContent("Internal Server Error", Encoding.UTF8);
                return serverError;
            }
        }
    }
}
