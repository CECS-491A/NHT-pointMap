using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Constants
{
    public class Constants
    {

        public Pages page { get; set; }

        public enum Pages
        {
            None,
            MapView,
            PointDetails,
            AdminDash,
            PointEditor
        }

        public enum Sources
        {
            None,
            Registration,
            Logout,
            Login,
            Mapview,
            PointDetails,
            AdminDash,
            PointEditor,
            Session,
            SSO
        }
    }
}
