﻿using System;
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
using System.Net;

namespace Testing.IntegrationTests
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
        public void ReadPoint_NonExisting_404()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var point = _tu.CreatePointObject(179, 81);

            var endpoint = API_ROUTE_LOCAL + "/api/point/" + point.Id;
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            ResponseMessageResult response = (ResponseMessageResult)controller.Get(point.Id.ToString());

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void ReadPoint_Unauthorized_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);

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

            ResponseMessageResult response =(ResponseMessageResult)controller.Get(point.Id.ToString());

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void ReadPoint_InvalidPayload_400()
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

            ResponseMessageResult response = (ResponseMessageResult)controller.Get(null);

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);
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
            ResponseMessageResult result404 = (ResponseMessageResult)controller.Get(point.Id.ToString());

            Assert.IsInstanceOfType(response, typeof(OkResult));
            Assert.AreEqual(result404.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void DeletePoint_Unauthorized_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);

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

            ResponseMessageResult response = (ResponseMessageResult)controller.Delete(point.Id.ToString());

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void DeletePoint_InvalidPayload_400()
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

            ResponseMessageResult response = (ResponseMessageResult)controller.Delete(null);

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void DeletePoint_NonExisting_404()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var point = _tu.CreatePointObject(179, 81);

            var endpoint = API_ROUTE_LOCAL + "/api/point/" + point.Id;
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            ResponseMessageResult response = (ResponseMessageResult)controller.Delete(point.Id.ToString());

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.NotFound);
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

            OkNegotiatedContentResult<Point> result = (OkNegotiatedContentResult<Point>)controller.Put(pointPost);

            Assert.AreEqual(pointPost.Name, result.Content.Name);
            Assert.AreEqual(pointPost.Description, result.Content.Description);
            Assert.AreEqual(pointPost.Longitude, result.Content.Longitude);
            Assert.AreEqual(pointPost.Latitude, result.Content.Latitude);
            Assert.AreEqual(pointPost.CreatedAt, result.Content.CreatedAt);
            Assert.AreNotEqual(pointPost.UpdatedAt, result.Content.UpdatedAt);
        }

        [TestMethod]
        public void UpdatePoint_Unauthorized_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);

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

            PointPOST pointPost = new PointPOST
            {
                Longitude = point.Longitude,
                Latitude = point.Latitude,
                Description = "updatedDescription",
                Name = "updatedName",
                CreatedAt = point.CreatedAt,
                UpdatedAt = point.UpdatedAt,
                Id = point.Id
            };

            ResponseMessageResult result = (ResponseMessageResult)controller.Put(pointPost);

            Assert.AreEqual(result.Response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void UpdatePoint_InvalidPayload_400()
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

            ResponseMessageResult result = (ResponseMessageResult)controller.Put(null);

            Assert.AreEqual(result.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void UpdatePoint_Invalid_Long_Lat_400()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var pointToPost = _tu.CreatePointObject(179, 81);
            var point = _tu.CreatePointInDb(pointToPost);

            var pointPost = new PointPOST
            {
                Name = point.Name,
                Description = point.Description,
                Longitude = 181,
                Latitude = point.Latitude
            };

            var endpoint = API_ROUTE_LOCAL + "/api/point/" + point.Id;
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            ResponseMessageResult response = (ResponseMessageResult)controller.Put(pointPost);

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);

            pointPost.Longitude = -181;

            response = (ResponseMessageResult)controller.Post(pointPost);
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);

            pointPost.Latitude = 91;

            response = (ResponseMessageResult)controller.Post(pointPost);
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);

            pointPost.Latitude = -91;

            response = (ResponseMessageResult)controller.Post(pointPost);
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void UpdatePoint_NonExisting_404()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var point = _tu.CreatePointObject(179, 81);

            var endpoint = API_ROUTE_LOCAL + "/api/point/" + point.Id;
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            PointPOST pointPost = new PointPOST
            {
                Longitude = point.Longitude,
                Latitude = point.Latitude,
                Description = "updatedDescription",
                Name = "updatedName",
                CreatedAt = point.CreatedAt,
                UpdatedAt = point.UpdatedAt,
                Id = point.Id
            };

            ResponseMessageResult result = (ResponseMessageResult)controller.Put(pointPost);

            Assert.AreEqual(result.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Create_Read_Update_Delete_201_200()
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
            NegotiatedContentResult<Point> response = (NegotiatedContentResult<Point>)controller.Post(pointPost);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Created);
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

            OkNegotiatedContentResult<Point> updateResponse = (OkNegotiatedContentResult<Point>)controller.Put(pointPost);

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

            ResponseMessageResult readResponse3 = (ResponseMessageResult)controller.Get(readResponse2.Content.Id.ToString());

            Assert.AreEqual(readResponse3.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void CreatePoint_201()
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

            NegotiatedContentResult<Point> response = (NegotiatedContentResult<Point>)controller.Post(point);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Created);
            Assert.AreEqual(point.Name, response.Content.Name);
            Assert.AreEqual(point.Description, response.Content.Description);
            Assert.AreEqual(point.Longitude, response.Content.Longitude);
            Assert.AreEqual(point.Latitude, response.Content.Latitude);
        }

        [TestMethod]
        public void CreatePoint_Unauthorized_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);

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

            ResponseMessageResult response = (ResponseMessageResult)controller.Post(point);

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void CreatePoint_InvalidPayload_400()
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

            ResponseMessageResult response = (ResponseMessageResult)controller.Post(null);

            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void CreatePoint_Invalid_Lat_Long_400()
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
                Longitude = 181,
                Latitude = 85,
                Description = "desc",
                Name = "name",
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            controller.Request = request;

            ResponseMessageResult response = (ResponseMessageResult)controller.Post(point);
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);

            point.Longitude = -181;

            response = (ResponseMessageResult)controller.Post(point);
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);

            point.Latitude = 91;

            response = (ResponseMessageResult)controller.Post(point);
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);

            point.Latitude = -91;

            response = (ResponseMessageResult)controller.Post(point);
            Assert.AreEqual(response.Response.StatusCode, HttpStatusCode.BadRequest);
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