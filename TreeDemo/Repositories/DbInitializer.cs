using System.Configuration;
using System.Data.SqlClient;

namespace Repositories
{
    public class DbInitializer
    {
        private readonly string _conStr;

        public DbInitializer()
        {
            _conStr = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }

        public void Initialize()
        {
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = sql;

                    cmd.CommandText = SqlQueries.InitBasicTables;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = SqlQueries.InitNodesTable;
                    cmd.ExecuteNonQuery();
                }
                sql.Close();
            }
        }
    }
}
