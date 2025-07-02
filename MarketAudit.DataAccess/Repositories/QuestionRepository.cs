using Dapper;
using Marketaudit.Entities.Models.Response;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Marketaudit.DataAccess.Repositories
{
    public class QuestionRepository : DataBaseRepository, IQuestionRepository
    {
        public QuestionRepository()
        {
            TABLE_NAME = "QUESTION";
        }


        public IEnumerable<QuestionModel> GetQuestionByProjectId(long projectId)
        {
            string query = string.Format("SELECT PQ.Id, q.Question, Q.Description, DT.Code as DataType, QT.Code as QuestionType, Q.Required, COALESCE(Q.Image,'null') AS Image, PQ.Orden " +
                "FROM Project_Questions PQ " +
                "JOIN {0} Q ON PQ.QuestionId = Q.Id " +
                "JOIN Data_Type DT on Q.DataTypeId = DT.Id " +
                "JOIN Question_Type QT on Q.QuestionTypeId = QT.Id " +
                "WHERE PQ.ProjectId = {1} " +
                "ORDER BY PQ.Orden", TABLE_NAME, projectId);

            var result = ExecuteQuery(query);

            List<QuestionModel> entities = new List<QuestionModel>();

            foreach (DataRow row in result)
            {
                QuestionModel itemRow = new QuestionModel();

                itemRow.Id = ToLong(row["Id"]);
                itemRow.Question = ToString(row["Question"]);
                itemRow.Description = ToString(row["Description"]);
                itemRow.DataType = ToString(row["DataType"]);
                itemRow.QuestionType = ToString(row["QuestionType"]);
                itemRow.Required = ToBoolean(row["Required"]);
                itemRow.QuestionImg = ToString(row["Image"]);
                itemRow.Order = ToInt(row["Orden"]);

                entities.Add(itemRow);
            }
            
            return entities;
        }

        public IEnumerable<ProjectQuestionTemplateModel> GetQuestionTemplateByProjectId(long projectId)
        {
            string query = @"
                SELECT DISTINCT Q.Id,
                                PQ.Orden,
                                Q.Question AS Pregunta,
                                Q.Description AS 'Descripcion',
                                Q.Image AS 'Miniatura',
                                CASE
                                    WHEN Q.Required = 1 THEN 'Si'
                                    ELSE 'No'
                                END AS 'Requerida',
                                COALESCE((SELECT STRING_AGG(CONCAT(R.Response, ';', QL.Value), '| ')
                                            FROM Question_Logic QL
                                            INNER JOIN Response R ON QL.ResponseId = R.Id
                                            WHERE QL.QuestionID = Q.Id
                                            GROUP BY QL.QuestionId), '') AS 'Disparadora',
                                QT.Code AS 'TipoPregunta',
                                DT.Code AS 'TipoDato',
                                COALESCE((SELECT STRING_AGG(R.Response, '; ')
                                            FROM Question_Responses QR
                                            INNER JOIN Response R ON QR.ResponseId = R.Id
                                            WHERE QR.QuestionID = Q.Id
                                            GROUP BY QR.QuestionId), '') AS 'Respuestas'
                FROM Project_Questions PQ
                JOIN Question Q ON PQ.QuestionId = Q.Id
                JOIN Question_Type QT ON Q.QuestionTypeId = QT.ID
                JOIN Data_Type DT ON Q.DataTypeId = DT.Id
                WHERE PQ.ProjectId = @ProjectId";

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();

            List<ProjectQuestionTemplateModel> entities;

            using (conn)
            {
                entities = conn.Query<ProjectQuestionTemplateModel>(query, new { ProjectId = projectId }).ToList();
                conn.Close();
            }

            return entities;
        }

        public IEnumerable<QuestionModel> GetQuestionByUserId(long userId)
        {
            string query = string.Format("SELECT PQ.Id, Q.Id As QuestionId, PQ.ProjectId, q.Question, Q.Description, DT.Code as DataType, QT.Code as QuestionType, Q.Required, Q.Image, PQ.Orden " +
                "FROM Project_Questions PQ " +
                "JOIN {0} Q ON PQ.QuestionId = Q.Id AND PQ.ProjectId IN (SELECT ProjectId FROM Route WHERE CensistId = {1}) " +
                "JOIN Data_Type DT on Q.DataTypeId = DT.Id " +
                "JOIN Question_Type QT on Q.QuestionTypeId = QT.Id ", TABLE_NAME, userId);

            var result = ExecuteQuery(query);

            List<QuestionModel> entities = new List<QuestionModel>();

            foreach (DataRow row in result)
            {
                QuestionModel itemRow = new QuestionModel();

                itemRow.Id = ToLong(row["Id"]);
                itemRow.QuestionId = ToLong(row["QuestionId"]);
                itemRow.ProjectId = ToLong(row["ProjectId"]);
                itemRow.Question = ToString(row["Question"]);
                itemRow.Description = ToString(row["Description"]);
                itemRow.DataType = ToString(row["DataType"]);
                itemRow.QuestionType = ToString(row["QuestionType"]);
                itemRow.Required = ToBoolean(row["Required"]);
                itemRow.QuestionImg = !string.IsNullOrEmpty(ToString(row["Image"])) ? ToString(row["Image"]) : null;
                itemRow.Order = ToInt(row["Orden"]);

                entities.Add(itemRow);
            }

            return entities.OrderBy(q => q.ProjectId).ThenBy(q => q.Order);
        }

        public long GetQuestions(long projectId)
        {
            string query = string.Format("select count(pq.Id) as ID from[Project_Questions] pq where pq.ProjectId = {1}", TABLE_NAME, projectId);

            var result = ExecuteQuery(query);

            var row = result[0];

            return ToLong(row["ID"]);

        }

        public bool Exist(string question, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT COUNT(ID) AS CONTADOR FROM {0} WHERE Question Like '{1}' ", TABLE_NAME, question);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToInt(row["CONTADOR"]) > 0;
        }

        public Question Get(string question, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT Id, Question, Description, QuestionTypeId, DataTypeId, Required, Image FROM {0} WHERE Question = '{1}' ", TABLE_NAME, question);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return new Question()
            {
                Id = ToLong(row["Id"]),
                QuestionName = ToString(row["Question"]),
                Description = ToString(row["Description"]),
                QuestionTypeId = ToLong(row["QuestionTypeId"]),
                DataTypeId = ToLong(row["DataTypeId"]),
                Required = ToBoolean(row["Required"]),
                Image = ToString(row["Image"])
            };
        }

        public long Insert(Question model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [{0}]  ([Question], [Description], [QuestionTypeId], [DataTypeId], [Required], [Image]) " +
                " VALUES ('{1}','{2}','{3}','{4}','{5}','{6}') select scope_identity() as id",
                TABLE_NAME, RemoveCharacterInvalid(model.QuestionName), RemoveCharacterInvalid(model.Description), model.QuestionTypeId, model.DataTypeId, model.Required? 1:0, model.Image);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToLong(row["id"]);
        }

        public void InsertQuestionResponse(QuestionResponse model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [Question_Responses]  ([QuestionId], [ResponseId]) " +
                " VALUES ('{0}','{1}')", model.QuestionId, model.ResponseId);

            ExecuteQuery(query, transaction);
        }

        public void InsertQuestionLogic(QuestionLogic model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [Question_Logic]  ([QuestionId], [ResponseId], [Value]) " +
                " VALUES ('{0}','{1}','{2}')", model.QuestionId, model.ResponseId, model.Value);

            ExecuteQuery(query, transaction);
        }

        public IEnumerable<Filter> GetQuestionFilter(long projectId, TransactionalContext transaction)
        {
            string query = string.Format("Select q.Id, q.Question from  Project_Questions p " +
                "join Question q on p.QuestionId = q.Id " +
                "where p.ProjectId = {0} and q.QuestionTypeId = 3", projectId);

            var result = ExecuteQuery(query);

            List<Filter> entities = new List<Filter>();

            foreach (DataRow row in result)
            {
                Filter itemRow = new Filter();

                itemRow.Key = ToLong(row["Id"]);
                itemRow.Value = ToString(row["Question"]);

                entities.Add(itemRow);
            }

            return entities;
        }

        public void DeleteQuestionProject(long projectId, ITransactionalContext transaction)
        {
            string query = string.Format("EXEC dbo.DeleteQuestion @ProjectId = {1}", TABLE_NAME, projectId);

            ExecuteQuery(query, transaction);
        }
    }
}
