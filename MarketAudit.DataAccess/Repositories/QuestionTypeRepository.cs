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
    public class QuestionTypeRepository : DataBaseRepository, IQuestionTypeRepository
    {
        public QuestionTypeRepository()
        {
            TABLE_NAME = "Question_Type";
        }

        public QuestionType Get(long id)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.Id = '{1}' ", TABLE_NAME, id);

            var result = ExecuteQuery(query);
            var row = result[0];
            return new QuestionType
            {
                Id = ToLong(row["Id"]),
                Code = ToString(row["Code"]),
                Descripcion = ToString(row["Description"])
            };
        }

        public QuestionType Get(string code, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.code = @code ", TABLE_NAME);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();

            QuestionType entities;

            using (conn)
            {
                entities = conn.QueryFirstOrDefault<QuestionType>(query, new { code });
                conn.Close();
            }

            if(entities != null)
            {
                return entities;
            }
            else
            {
                throw new Exception(string.Format("El tipo de pregunta {0} no existe", code));
            }
        }

        public IEnumerable<QuestionType> GetAll()
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R ", TABLE_NAME);

            var result = ExecuteQuery(query);

            List<QuestionType> entities = new List<QuestionType>();

            foreach (DataRow row in result)
            {
                QuestionType itemRow = new QuestionType
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
