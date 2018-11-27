using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class DbUtility
    {
        public static string GetTimeStampUTC()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-Thh:mm:ss");

        }
    }
}
