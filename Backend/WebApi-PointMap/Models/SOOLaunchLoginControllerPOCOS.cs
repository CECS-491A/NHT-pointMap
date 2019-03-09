using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_PointMap.Models
{
    public class SOOLaunchLoginControllerPOCOS
    {
    }

    public class LoginPOCO
    {
        public string SSOUserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class ResponsePOCO
    {
        public Object Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

   
        
    
}