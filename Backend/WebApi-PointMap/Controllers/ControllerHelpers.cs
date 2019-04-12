using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebApi_PointMap.Controllers
{
    public class ControllerHelpers
    {
        public static string GetAndCheckToken(HttpRequestMessage request, string header)
        {
            var token = GetHeader(request, "Token");

            if (token.Length < 1)
            {
                throw new HttpRequestException("No token provided.");
            }
            return token;
        }

        public static string GetHeader(HttpRequestMessage request, string header)
        {
            IEnumerable<string> headerValues;
            var nameFilter = string.Empty;
            if (request.Headers.TryGetValues(header, out headerValues))
            {
                nameFilter = headerValues.FirstOrDefault();
            }
            return nameFilter;
        }

        public static Guid ParseAndCheckId(string id)
        {
            Guid guid;

            // check if valid SSO ID format
            var validParse = Guid.TryParse(id, out guid);
            if (!validParse)
            {
                throw new FormatException("Invalid User Id.");
            }
            return guid;
        }

        public static Session ValidateAndUpdateSession(DatabaseContext _db, string token)
        {
            AuthorizationManager _authorizationManager = new AuthorizationManager();
            var session = _authorizationManager.ValidateAndUpdateSession(_db, token);
            if (session == null)
            {
                throw new NullReferenceException("Session is no longer available.");
            }
            return session;
        }
    }
}