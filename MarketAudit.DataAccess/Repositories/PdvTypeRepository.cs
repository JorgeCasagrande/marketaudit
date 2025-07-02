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
    public class PdvTypeRepository : DataBaseRepository, IPdvTypeRepository
    {
        public PdvTypeRepository()
        {
            TABLE_NAME = "Pdv_Type";
        }

        public PdvType Get(long id)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.Id = '{1}' ", TABLE_NAME, id);

            var result = ExecuteQuery(query);
            var row = result[0];
            return new PdvType
            {
                Id = ToLong(row["Id"]),
                Code = ToString(row["Code"]),
                Descripcion = ToString(row["Description"])
            };
        }

        public PdvType Get(string description, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.Description = @description ", TABLE_NAME);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();

            PdvType entities;

            using (conn)
            {
                entities = conn.QueryFirstOrDefault<PdvType>(query, new { description });
                conn.Close();
            }

            return entities;
        }

        public IEnumerable<PdvType> GetAll()
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R ", TABLE_NAME);

            var result = ExecuteQuery(query);

            List<PdvType> entities = new List<PdvType>();

            foreach (DataRow row in result)
            {
                PdvType itemRow = new PdvType
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
