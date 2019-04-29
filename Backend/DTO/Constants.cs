using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Constants
{
    /// <summary>
    /// Holds constant enumerations and values to be accessed from other classes
    /// </summary>
    public class Constants
    {
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
