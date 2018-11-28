using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class Constants
    {
        public static string getDbConnectionString() {
            string db_name = System.Environment.GetEnvironmentVariable("POINTMAP_DB_NAME");
            if (db_name == null) db_name = "pointmap";

            string db_host = System.Environment.GetEnvironmentVariable("POINTMAP_DB_HOST");
            if (db_host == null) db_host = "localhost";

            string db_user = System.Environment.GetEnvironmentVariable("POINTMAP_DB_USER");
            if (db_user == null) db_user = "pointmap";

            string db_pass = System.Environment.GetEnvironmentVariable("POINTMAP_DB_PASS");
            if (db_pass == null) db_pass = "pointmap";

            string db_port = System.Environment.GetEnvironmentVariable("POINTMAP_DB_PORT");
            if (db_port == null) db_port = "5432";

            string conn = "User ID=" + db_user + ";Password=" + db_pass + ";Server=" + db_host + ";Port=" + db_port + ";Database=" + db_name + ";Integrated Security=true;Pooling=true;";

            return conn;
        }
    }
}
