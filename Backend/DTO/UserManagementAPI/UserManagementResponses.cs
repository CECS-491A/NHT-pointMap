using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.UserManagementAPI
{
    public class UserManagementResponses
    {
    }

    public class GetAllUsersResponseDataItem
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public Guid? manager { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public bool disabled { get; set; }
        public bool isAdmin { get; set; }
    }

    public class GetUserResponseData
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public bool disabled { get; set; }
        public bool isAdmin { get; set; }
    }
}
