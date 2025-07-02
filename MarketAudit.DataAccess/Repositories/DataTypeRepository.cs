using Dapper;
using Marketaudit.Entities.Models.Response;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Marketaudit.DataAccess.Repositories
{
    public class DataTypeRepository : DataBaseRepository, IDataTypeRepository
    {
        public DataTypeRepository()
        {
            TABLE_NAME = "Data_Type";
        }

        public DataType Get(long id)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.Id = '{1}' ", TABLE_NAME, id);

            var result = ExecuteQuery(query);
            var row = result[0];
            return new DataType
            {
                Id = ToLong(row["Id"]),
                Code = ToString(row["Code"]),
                Descripcion = ToString(row["Description"])
            };
        }

        public DataType Get(string code, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.code = @code ", TABLE_NAME);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();

            DataType entities;

            using (conn)
            {
                entities = conn.QueryFirstOrDefault<DataType>(query, new { code });
                conn.Close();
            }

            if (entities != null)
            {
                return entities;
            }
            else
            {
                throw new Exception(string.Format("El tipo de dato {0} no existe", code));
            }
        }

        public IEnumerable<DataType> GetAll()
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R ", TABLE_NAME);

            var result = ExecuteQuery(query);

            List<DataType> entities = new List<DataType>();

            foreach (DataRow row in result)
            {
                DataType itemRow = new DataType
                {
                    Id = ToLong(row["Id"]),
                    Code = ToString(row["Code"]),
                    Descripcion = ToString(row["Description"])
                };

                entities.Add(itemRow);
            }

            return entities;
        }
    }
}
