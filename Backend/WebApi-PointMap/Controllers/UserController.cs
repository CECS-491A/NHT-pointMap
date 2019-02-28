using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi_PointMap.Models;

namespace WebApi_PointMap.Controllers
{
    [EnableCors(origins: "http://pointmap.me:80", headers: "*", methods: "*")]
    public class UserController : ApiController
    {

    }
}
