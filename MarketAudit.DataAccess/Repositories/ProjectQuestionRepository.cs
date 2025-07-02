using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Marketaudit.DataAccess.Repositories
{
    public class ProjectQuestionRepository : DataBaseRepository, IProjectQuestionRepository
    {
        public ProjectQuestionRepository()
        {
            TABLE_NAME = "Project_Questions";
        }

        public DataTable GetColumnsReport(long projectId, ITransactionalContext context)
        {
            string query = string.Format("select q.[Description] As Question, " +
                    " mq.[Orden] from {0} mq" +
                    " join question q on mq.QuestionId = q.Id" +
                    " where mq.ProjectId = {1} " +
                    " order by mq.[Orden]", TABLE_NAME, projectId);

            var result = ExecuteQuery(query, context);

            DataTable dt = new DataTable();
            dt.Columns.Add("Question");
            dt.Columns.Add("Order");
            
            foreach (DataRow item in result)
            {
                DataRow row = dt.NewRow();
                row[0] = Convert.ToString(item["Question"]);
                row[1] = Convert.ToString(item["Orden"]);
                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
