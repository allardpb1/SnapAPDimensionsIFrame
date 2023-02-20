using System.Data.SqlClient;

namespace SnapAPDimensionsIFrame
{
    public static class MainModule
    {
        public static string GetDBConnectionString(IConfiguration _config)
        {
            string strConn;
            string strServer = _config.GetValue<string>("SqlServerName");
            string strDB = _config.GetValue<string>("SqlDatabase");
            string strUser = _config.GetValue<string>("SqlUserID");
            string strPw = _config.GetValue<string>("SqlPassword");

            try
            {
                strConn = "" + "Server=" + strServer + ";Initial Catalog=" + strDB + ";";
                strConn += "User ID=" + strUser + ";Password=" + strPw;
                return strConn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static SqlConnection GetDBConnection(string strConn, bool openConnection = false)
        {
            SqlConnection sqlConn = new SqlConnection();
            sqlConn.ConnectionString = strConn;
            if (openConnection)
            {
                try
                {
                    sqlConn.Open();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return sqlConn;
        }
    }
}
