using MarketAudit.Common.Exceptions;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.Common.Log;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Marketaudit.DataAccess.Repositories
{
    public class DataBaseRepository : BaseRepository
    {
        protected override DbConnection GetNewConnection()
        {
            return new SqlConnection(databaseConnectionString);
        }

        protected override void ConfigureConnectionString()
        {
            databaseConnectionString = GlobalVariables.GetDatabaseConnectionString();
        }

        protected override DataRowCollection ExecuteQuery(string query, ITransactionalContext transactionalContext = null)
        {
            //var logger = new LoggerManager();
            //logger.LogInfo(string.Format("Query Executed: {0}", query));
            SqlCommand command;
            if (transactionalContext == null)
            {
                command = new SqlCommand(query, (SqlConnection)GetNewConnection());
                command.CommandTimeout = 300;
                command.CommandType = CommandType.Text;
            }
            else
            {
                command = transactionalContext.GetCommand();
                command.CommandTimeout = 300;
                command.CommandText = query;
            }

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.ReturnProviderSpecificTypes = true;
            try
            {
                return ExecuteDataSet(adapter, transactionalContext != null);
            }
            catch (Exception e)
            {
                throw new DatabaseQueryExecutionException(e.Message);
            }
        }

        protected override DataRowCollection ExecuteStoreProcedure(string query, List<Parametros> parametros, ITransactionalContext transactionalContext = null)
        {
            //var logger = new LoggerManager();
            //logger.LogInfo(string.Format("Store Procedure: {0}", query));
            SqlCommand command;
            if (transactionalContext == null)
            {
                command = new SqlCommand(query, (SqlConnection)GetNewConnection());
                command.CommandType = CommandType.StoredProcedure;
                foreach(var item in parametros)
                {
                    command.Parameters.Add(new SqlParameter( item.Parametro, item.Valor));
                    //logger.LogInfo(string.Format("Parameters: {0} : {1}", item.Parametro, item.Valor));
                }
            }
            else
            {
                command = transactionalContext.GetCommand();
                command.CommandType = CommandType.StoredProcedure;
                foreach (var item in parametros)
                {
                    command.Parameters.Add(new SqlParameter(item.Parametro, item.Valor));
                    //logger.LogInfo(string.Format("Parameters: {0} : {1}", item.Parametro, item.Valor));
                }
                command.CommandText = query;
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.ReturnProviderSpecificTypes = true;
            try
            {
                return ExecuteDataSet(adapter, transactionalContext != null);
            }
            catch (Exception e)
            {
                throw new DatabaseQueryExecutionException(e.Message);
            }
        }

        protected override string ToString(object value)
        {
            try
            {
                SqlString strValue = (SqlString)value;
                if (strValue.IsNull)
                {
                    return null;
                }
                return strValue.Value;
            }
            catch (Exception e)
            {
                return value.ToString();
            }
        }

        protected decimal ToDecimal(object value)
        {
            if (((INullable)value).IsNull)
            {
                return 0;
            }
            return decimal.Parse(value.ToString());
        }

        protected int ToInt(object value)
        {
            if (((INullable)value).IsNull)
            {
                return 0;
            }
            return int.Parse(value.ToString());
        }
        protected int? ToNullableInt(object value)
        {
            if (((INullable)value).IsNull)
            {
                return null;
            }
            return ToInt(value);
        }

        protected long ToLong(object value)
        {
            if (((INullable)value).IsNull)
            {
                return 0;
            }
            return long.Parse(value.ToString());
        }

        protected long? ToNullableLong(object value)
        {
            if (((INullable)value).IsNull)
            {
                return null;
            }
            return ToLong(value);
        }

        protected bool ToBoolean(object value)
        {
            if (((INullable)value).IsNull)
            {
                return false;
            }
            return Boolean.Parse(value.ToString());
        }

        protected decimal? ToNullableDecimal(object value)
        {
            if (((INullable)value).IsNull)
            {
                return null;
            }
            return decimal.Parse(value.ToString());
        }

        protected int FromBooleanToBit(bool value)
        {
            return value ? 1 : 0;
        }

        protected string FormatQueryString(string original)
        {
            return original.Replace("\'", "\'\'");
        }

        protected DateTime ToDateTime(object value)
        {
            if (((INullable)value).IsNull)
            {
                return DateTime.Now;
            }
            return DateTime.Parse(value.ToString());
        }

        protected DateTime? ToNullableDateTime(object value)
        {
            if (((INullable)value).IsNull)
            {
                return null;
            }
            return DateTime.Parse(value.ToString());
        }

        protected string FromStringNullable(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "NULL";
            }
            else
            {
                return "'" + value + "'";
            }
        }

        protected string RemoveCharacterInvalid(string value)
        {
            if(!string.IsNullOrEmpty(value))
            {
                return value.Replace("'", " ");
            }

            return value;
        }

        protected List<string> ReadFileLines(string path)
        {
            List<string> fileLines = new List<string>();
            if (!File.Exists(path))
                return fileLines;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    fileLines.Add(sr.ReadLine());
                }

            }
            return fileLines;
        }
    }
}
