using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PointMap.Services;

namespace PointMap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public UsersController(IPasswordService passwordService) {
            _passwordService = passwordService;
        }

        // POST api/users/register
        [HttpPost("register")]
        public ActionResult<Byte[]> Register([FromBody] string request)
        {
            byte[] salt = _passwordService.GenerateSalt();

            string hash = _passwordService.HashPassword("a", salt); // This should be stored in the database.

            byte[] token = _passwordService.GenerateSalt();

            return token;
        }

        // POST api/users/login
        [HttpPost("login")]
        public void Login([FromBody] string request)
        {

        }
    }
}
