using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_PointMap.Models
{
    public class PointPOST
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}