﻿using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System;
using System.Data.Entity.Validation;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLayer.AccessControl
{
    class AuthorizationManager
    {
        private ISessionService _sessionService;
        private IUserService _userService;

        private DatabaseContext CreateDbContext()
        {
            return new DatabaseContext();
        }

        public AuthorizationManager()
        {
            _sessionService = new SessionService();
            _userService = new UserService();
        }

        public string GenerateSessionToken()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Byte[] b = new byte[64 / 2];
            provider.GetBytes(b);
            string hex = BitConverter.ToString(b).Replace("-", "");
            return hex;
        }

        public Session CreateSession(DatabaseContext _db, string userEmail)
        {
            var userResponse = _userService.GetUser(_db, userEmail);
            if(userResponse == null)
            {
                return null;
            }
            Session session = new Session();
            session.Token = GenerateSessionToken();
            session.User = userResponse;

            var response = _sessionService.CreateSession(_db, session, userResponse.Id);

            return response;
        }

        public Session ValidateAndUpdateSession(DatabaseContext _db, string token)
        {
            var response = _sessionService.ValidateSession(_db, token);

            if(response != null)
            {
                response = _sessionService.UpdateSession(_db, response);
            }
            else
            {
                return null;
            }
            return response;
        }

        public Session ExpireSession(DatabaseContext _db, string token)
        {
            return _sessionService.ExpireSession(_db, token);
        }
    }
}
