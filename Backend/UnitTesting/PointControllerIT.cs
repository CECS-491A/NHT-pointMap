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
        DatabaseContext _db;

        public PointControllerIT()
        {
            controller = new PointController();
            _tu = new TestingUtils();
            _db = _tu.CreateDataBaseContext();
            _ss = new SessionService(_db);
        }

        [TestMethod]
        public void ReadPoint_200()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var point = _tu.CreatePointObject(179, 81);
            point = _tu.CreatePointInDb(point);

            var endpoint = API_ROUTE_LOCAL + "/api/point/" + point.Id;
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            OkNegotiatedContentResult<Point> response = (OkNegotiatedContentResult<Point>)controller.Get(point.Id.ToString());

            Assert.AreEqual(point.Name, response.Content.Name);
            Assert.AreEqual(point.Description, response.Content.Description);
            Assert.AreEqual(point.Longitude, response.Content.Longitude);
            Assert.AreEqual(point.Latitude, response.Content.Latitude);
        }

        [TestMethod]
        public void DeletePoint_200()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var point = _tu.CreatePointObject(179, 81);
            point = _tu.CreatePointInDb(point);

            var endpoint = API_ROUTE_LOCAL + "/api/point/" + point.Id;
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            OkResult response = (OkResult)controller.Delete(point.Id.ToString());
            OkNegotiatedContentResult<Point> expectedNullResponse = (OkNegotiatedContentResult<Point>)controller.Get(point.Id.ToString());

            Assert.IsInstanceOfType(response, typeof(OkResult));
            Assert.IsNull(expectedNullResponse.Content);
        }

        [TestMethod]
        public void UpdatePoint_200()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var point = _tu.CreatePointObject(179, 81);
            point = _tu.CreatePointInDb(point);

            var endpoint = API_ROUTE_LOCAL + "/api/point/" + point.Id;
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            OkNegotiatedContentResult<Point> response = (OkNegotiatedContentResult<Point>)controller.Get(point.Id.ToString());

            PointPOST pointPost = new PointPOST
            {
                Longitude = response.Content.Longitude,
                Latitude = response.Content.Latitude,
                Description = "updatedDescription",
                Name = "updatedName",
                CreatedAt = response.Content.CreatedAt,
                UpdatedAt = response.Content.UpdatedAt,
                Id = response.Content.Id
            };

            Assert.AreEqual(point.Name, response.Content.Name);
            Assert.AreEqual(point.Description, response.Content.Description);
            Assert.AreEqual(point.Longitude, response.Content.Longitude);
            Assert.AreEqual(point.Latitude, response.Content.Latitude);

            OkNegotiatedContentResult<Point> result = (OkNegotiatedContentResult<Point>)controller.Put(pointPost.Id.ToString(), pointPost);

            Assert.AreEqual(pointPost.Name, result.Content.Name);
            Assert.AreEqual(pointPost.Description, result.Content.Description);
            Assert.AreEqual(pointPost.Longitude, result.Content.Longitude);
            Assert.AreEqual(pointPost.Latitude, result.Content.Latitude);
            Assert.AreEqual(pointPost.CreatedAt, result.Content.CreatedAt);
            Assert.AreNotEqual(pointPost.UpdatedAt, result.Content.UpdatedAt);
        }

        [TestMethod]
        public void Create_Read_Update_Delete_200()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var createEndpoint = API_ROUTE_LOCAL + "/api/point";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(createEndpoint)
            };

            PointPOST pointPost = new PointPOST
            {
                Longitude = 179,
                Latitude = 85,
                Description = "desc",
                Name = "name",
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            //Create Test
            OkNegotiatedContentResult<Point> response = (OkNegotiatedContentResult<Point>)controller.Post(pointPost);

            Assert.AreEqual(pointPost.Name, response.Content.Name);
            Assert.AreEqual(pointPost.Description, response.Content.Description);
            Assert.AreEqual(pointPost.Longitude, response.Content.Longitude);
            Assert.AreEqual(pointPost.Latitude, response.Content.Latitude);

            //Read Test1
            OkNegotiatedContentResult<Point> readResponse = (OkNegotiatedContentResult<Point>)controller.Get(response.Content.Id.ToString());

            Assert.AreEqual(response.Content.Name, readResponse.Content.Name);
            Assert.AreEqual(response.Content.Description, readResponse.Content.Description);
            Assert.AreEqual(response.Content.Longitude, readResponse.Content.Longitude);
            Assert.AreEqual(response.Content.Latitude, readResponse.Content.Latitude);
            Assert.AreEqual(response.Content.CreatedAt, readResponse.Content.CreatedAt);
            Assert.AreEqual(response.Content.UpdatedAt, readResponse.Content.UpdatedAt);

            pointPost = new PointPOST
            {
                Longitude = readResponse.Content.Longitude,
                Latitude = response.Content.Latitude,
                Description = "updatedDescription",
                Name = "updatedName",
                CreatedAt = readResponse.Content.CreatedAt,
                UpdatedAt = readResponse.Content.UpdatedAt,
                Id = readResponse.Content.Id
            };

            OkNegotiatedContentResult<Point> updateResponse = (OkNegotiatedContentResult<Point>)controller.Put(pointPost.Id.ToString(), pointPost);

            Assert.AreEqual(pointPost.Name, updateResponse.Content.Name);
            Assert.AreEqual(pointPost.Description, updateResponse.Content.Description);
            Assert.AreEqual(pointPost.Longitude, updateResponse.Content.Longitude);
            Assert.AreEqual(pointPost.Latitude, updateResponse.Content.Latitude);
            Assert.AreEqual(pointPost.CreatedAt, updateResponse.Content.CreatedAt);
            Assert.AreNotEqual(pointPost.UpdatedAt, updateResponse.Content.UpdatedAt);

            //Read Test2
            OkNegotiatedContentResult<Point> readResponse2 = (OkNegotiatedContentResult<Point>)controller.Get(updateResponse.Content.Id.ToString());

            Assert.AreEqual(updateResponse.Content.Name, readResponse2.Content.Name);
            Assert.AreEqual(updateResponse.Content.Description, readResponse2.Content.Description);
            Assert.AreEqual(updateResponse.Content.Longitude, readResponse2.Content.Longitude);
            Assert.AreEqual(updateResponse.Content.Latitude, readResponse2.Content.Latitude);
            Assert.AreEqual(updateResponse.Content.CreatedAt, readResponse2.Content.CreatedAt);
            Assert.AreEqual(updateResponse.Content.UpdatedAt, readResponse2.Content.UpdatedAt);

            OkResult deleteResponse = (OkResult)controller.Delete(readResponse2.Content.Id.ToString());

            Assert.IsInstanceOfType(deleteResponse, typeof(OkResult));

            OkNegotiatedContentResult<Point> readResponse3 = (OkNegotiatedContentResult<Point>)controller.Get(readResponse2.Content.Id.ToString());

            Assert.IsNull(readResponse3.Content);
        }

        [TestMethod]
        public void CreatePoint_200()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/api/point";
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            PointPOST point = new PointPOST
            {
                Longitude = 179,
                Latitude = 85,
                Description = "desc",
                Name = "name",
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            OkNegotiatedContentResult<Point> response = (OkNegotiatedContentResult<Point>)controller.Post(point);

            Assert.AreEqual(point.Name, response.Content.Name);
            Assert.AreEqual(point.Description, response.Content.Description);
            Assert.AreEqual(point.Longitude, response.Content.Longitude);
            Assert.AreEqual(point.Latitude, response.Content.Latitude);
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
