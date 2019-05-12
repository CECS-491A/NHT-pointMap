<<<<<<< HEAD
﻿using System;
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
=======
﻿using System;
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
            LegalAndPrivacy,
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
>>>>>>> 3f4086a1d7e2957189c7a2d342adc522f1e60936
