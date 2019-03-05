using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_PointMap.Models
{
    // poco class for request object
    public class UserPOSTDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginDTO
    {
        public string SSOUserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class ResponseDTO
    {
        public Object Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}