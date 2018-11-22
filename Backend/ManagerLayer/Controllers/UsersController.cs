using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer;

namespace ManagerLayer
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public UsersController()
        {
            _passwordService = new PasswordService();
        }

        // POST api/users/register
        [HttpPost("register")]
        public ActionResult<Byte[]> Register([FromBody] Credentials data)
        {
            byte[] salt = _passwordService.GenerateSalt();
            string hash = _passwordService.HashPassword(data.Password, salt); // This should be stored in the database.
            byte[] token = _passwordService.GenerateSalt();
            Console.WriteLine(data.Username + "  " + data.Password);
            return token;
        }

        // POST api/users/login
        [HttpPost("login")]
        public void Login([FromBody] string request)
        {

        }
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
