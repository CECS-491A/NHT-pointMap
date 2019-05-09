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
            None = 0,
            MapView,
            PointDetails,
            AdminDash,
            PointEditor,
            Policies,
            FAQ,
            Account
        }

        public enum Sources
        {
            None = 0,
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
