using MarketAudit.Common.Exceptions;
using MarketAudit.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace MarketAudit.DataAccess.Repositories
{
    public class BaseRepository
    {
        protected string databaseConnectionString;
        protected string TABLE_NAME;

        public BaseRepository()
        {
            ConfigureConnectionString();
            TABLE_NAME = string.Empty;
        }

        protected virtual DbConnection GetNewConnection()
        {
            return null;
        }

        protected virtual void ConfigureConnectionString()
        {
            databaseConnectionString = string.Empty;
        }

        protected DataRowCollection ExecuteDataSet(DbDataAdapter adapter, bool isTransaction = true)
        {
            var dataTable = new DataTable();
            var returnValue = new DataSet();
            try
            {
                if (!isTransaction)
                {
                    adapter.SelectCommand.Connection.Open();
                }
            }
            catch (Exception e)
            {
                throw new DatabaseConnectionException(e.Message);
            }

            try
            {
                adapter.Fill(dataTable);
                returnValue.Tables.Add(dataTable);
                if (!isTransaction)
                {
                    adapter.SelectCommand.Connection.Close();
                }
                return returnValue.Tables[0].Rows;
            }
            catch (Exception e)
            {
                adapter.SelectCommand.Connection.Close();
                throw new DatabaseQueryExecutionException(e.Message);
            }
        }

        protected virtual DataRowCollection ExecuteQuery(string query, ITransactionalContext transactionalContext = null)
        {
            return null;
        }

        protected virtual DataRowCollection ExecuteStoreProcedure(string query, List<Parametros> parametros, ITransactionalContext transactionalContext = null)
        {
            return null;
        }

        protected bool AreAllColumnsEmpty(DataRow dr)
        {
            if (dr == null)
            {
                return true;
            }
            else
            {
                foreach (var value in dr.ItemArray)
                {
                    if (value != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        protected virtual string ToString(object value)
        {
            return value.ToString();
        }
    }

}
