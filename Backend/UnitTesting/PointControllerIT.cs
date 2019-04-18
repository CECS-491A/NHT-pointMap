using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApi_PointMap.Controllers;
using DataAccessLayer.Models;
using System.Net.Http;
using System.Web.Http;
using WebApi_PointMap.Models;
using ServiceLayer.Services;
using DataAccessLayer.Database;
using System.Threading;
using System.Web.Http.Results;

namespace UnitTesting
{
    [TestClass]
    public class PointControllerIT
    {
        const string API_ROUTE_LOCAL = "http://localhost:58896";
        PointController controller;
        TestingUtils _tu;
        User newUser;
        SessionService _ss;

        public PointControllerIT()
        {
            controller = new PointController();
            _tu = new TestingUtils();
            _ss = new SessionService();
        }

        [TestMethod]
        public void getAllPoints200()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            //Uncomment to add mock data
            //for (int i = 1; i < 11; i++)
            //{
            //    float num = (float)(i * .1);
            //    Point newPoint = _tu.CreatePointObject(10 + num, -1 * (10 + num));
            //    _tu.CreatePointInDb(newPoint);
            //    Console.WriteLine(newPoint.Id);
            //}


            var endpoint = API_ROUTE_LOCAL + "/api/points/";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("minLng", "10");
            request.Headers.Add("maxLng", "11");
            request.Headers.Add("minLat", "-11");
            request.Headers.Add("maxLat", "-10");
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            var response = controller.GetPoints();

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
        }
    }
}
