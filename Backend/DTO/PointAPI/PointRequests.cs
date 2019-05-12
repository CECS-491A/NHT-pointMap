using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.PointAPI
{
    public class PointCreateDTO
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class PointUpdateDTO
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
