using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Marketaudit.DataAccess.Repositories
{
    public class ResponseRepository : DataBaseRepository, IResponseRepository
    {
        public ResponseRepository()
        {
            TABLE_NAME = "RESPONSE";
        }

        public bool Exist(string response, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT COUNT(ID) AS CONTADOR FROM {0} WHERE Response = '{1}' ", TABLE_NAME, RemoveCharacterInvalid(response));

            var result = ExecuteQuery(query,transaction);

            var row = result[0];

            return ToInt(row["CONTADOR"]) > 0;
        }

        public ResponseQuestion Get(string response, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT R.Id, R.Response, R.Icon FROM {0} R where R.Response = '{1}' ", TABLE_NAME, RemoveCharacterInvalid(response));

            var result = ExecuteQuery(query,transaction);
            var row = result[0];
            return new ResponseQuestion
            {
                Id = ToLong(row["Id"]),
                Response = ToString(row["Response"]),
                Icon = ToString(row["Icon"])
            };
        }

        public IEnumerable<Responses> GetResponsesByQuestionId(long projectId, long questionId)
        {
            string query = string.Format("SELECT R.Id, QR.QuestionId, R.Response, " +
                "CASE " +
                "WHEN COALESCE(Value, 0) > 0 " +
                "THEN(SELECT PQ2.QuestionId FROM Project_Questions PQ2 WHERE PQ2.ProjectId = PQ.ProjectId AND PQ2.Orden = (PQ.Orden + QL.Value + 1)) " +
                "ELSE COALESCE(Value, 0) END As ValueLogic " +
                "FROM Question_Responses QR " +
                "JOIN {0} R on QR.ResponseId = R.Id " +
                "JOIN Project_Questions PQ ON QR.QuestionId = PQ.QuestionId " +
                "LEFT JOIN Question_Logic QL on QR.QuestionId = QL.QuestionId AND QR.ResponseId = QL.ResponseId " +
                "WHERE PQ.ProjectId = {1} AND QR.QuestionId = {2}", TABLE_NAME, projectId, questionId);

            var result = ExecuteQuery(query);

            List<Responses> entities = new List<Responses>();

            foreach (DataRow row in result)
            {
                Responses itemRow = new Responses();

                itemRow.Id = ToLong(row["Id"]);
                itemRow.QuestionId = ToLong(row["QuestionId"]);
                itemRow.Response = ToString(row["Response"]);
                itemRow.ValueLogic = ToInt(row["ValueLogic"]);

                entities.Add(itemRow);
            }
            
            return entities;
        }

        public IEnumerable<Responses> GetResponsesByUserId(long userId)
        {
            string query = string.Format("SELECT DISTINCT QR.ID, R.Id AS ResponseId, QR.QuestionId, R.Response, r.Icon, " +
                "CASE " +
                "WHEN COALESCE(Value, 0) > 0 " +
                "THEN(SELECT PQ2.ID FROM Project_Questions PQ2 WHERE PQ2.ProjectId = PQ.ProjectId AND PQ2.Orden = (PQ.Orden + QL.Value + 1)) " +
                "ELSE COALESCE(Value, 0) END As ValueLogic " +
                "FROM Question_Responses QR " +
                "JOIN {0} R on QR.ResponseId = R.Id " +
                "JOIN Project_Questions PQ ON QR.QuestionId = PQ.QuestionId " +
                "JOIN Route RO ON PQ.ProjectId = RO.ProjectId " +
                "LEFT JOIN Question_Logic QL on QR.QuestionId = QL.QuestionId AND QR.ResponseId = QL.ResponseId " +
                "WHERE RO.CensistId = {1} ORDER BY QR.ID", TABLE_NAME, userId);

            var result = ExecuteQuery(query);

            List<Responses> entities = new List<Responses>();

            foreach (DataRow row in result)
            {
                Responses itemRow = new Responses();

                itemRow.Id = ToLong(row["ResponseId"]);
                itemRow.QuestionId = ToLong(row["QuestionId"]);
                itemRow.Response = ToString(row["Response"]);
                itemRow.ValueLogic = ToInt(row["ValueLogic"]);
                itemRow.IconResponse = ToString(row["Icon"]);

                entities.Add(itemRow);
            }

            return entities;
        }

        public long Insert(ResponseQuestion model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [{0}]  ([Response], [Icon]) " +
          " VALUES ('{1}','{2}') select scope_identity() as id",
          TABLE_NAME, RemoveCharacterInvalid(model.Response), model.Icon);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToLong(row["id"]);
        }
    }
}
