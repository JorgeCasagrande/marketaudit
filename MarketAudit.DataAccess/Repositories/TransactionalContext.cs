using MarketAudit.Common.GlobalVariables;
using MarketAudit.DataAccess.Interfaces;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace MarketAudit.DataAccess.Repositories
{
    public class TransactionalContext : ITransactionalContext
    {
        private DbConnection dbConnection;
        public string connectionString;
        private SqlCommand cmd;
        private IDbTransaction transaction;
        public TransactionalContext()
        {
            connectionString = GlobalVariables.GetDatabaseConnectionString();
            dbConnection = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            Begin();
        }

        private void Begin()
        {
            dbConnection.Open();
            transaction = dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Commit()
        {
            if (!dbConnection.State.Equals(ConnectionState.Closed))
            {
                transaction.Commit();
                dbConnection.Close();
            }
        }

        public void Rollback()
        {
            if (!dbConnection.State.Equals(ConnectionState.Closed))
            {
                transaction.Rollback();
                dbConnection.Close();
            }
        }

        public SqlCommand GetCommand()
        {
            cmd.Parameters.Clear();
            cmd.Connection = (SqlConnection)dbConnection;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Transaction = (SqlTransaction)transaction;
            return cmd;
        }
    }
}
