
namespace MarketAudit.Common.GlobalVariables
{
    public static class GlobalVariables
    {
        private static string databaseConnectionString;
        private static string reportDatabaseConnectionString;

        public static string GetDatabaseConnectionString()
        {
            string connStr = databaseConnectionString;

            return connStr;
        }

        public static void SetDatabaseConnectionString(string connString)
        {
            databaseConnectionString = connString;
        }

        public static string GetReportDatabaseConnectionString()
        {
            string connStr = reportDatabaseConnectionString;

            return connStr;
        }

        public static void SetReportDatabaseConnectionString(string connString)
        {
            reportDatabaseConnectionString = connString;
        }

        public static string LogsPath;
        public static string CurrentDirectory;
    }
}
