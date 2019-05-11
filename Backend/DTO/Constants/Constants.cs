using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Constants
{
    public class Constants
    {
        public enum Pages
        {
            None = 0,
            MapView,
            PointDetails,
            AdminDashboard,
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
            AdminDashboard,
            PointEditor,
            Session,
            SSO
        }
    }
}
