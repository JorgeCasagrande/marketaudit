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
    public class ProjectSizeRepository : DataBaseRepository, IProjectSizeRepository
    {
        public ProjectSizeRepository()
        {
            TABLE_NAME = "Project_Size";
        }

        public ProjectSize Get(long id)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.Id = '{1}' ", TABLE_NAME, id);

            var result = ExecuteQuery(query);
            var row = result[0];
            return new ProjectSize
            {
                Id = ToLong(row["Id"]),
                Code = ToString(row["Code"]),
                Descripcion = ToString(row["Description"])
            };
        }

        public IEnumerable<ProjectSize> GetAll()
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R ", TABLE_NAME);

            var result = ExecuteQuery(query);

            List<ProjectSize> entities = new List<ProjectSize>();

            foreach (DataRow row in result)
            {
                ProjectSize itemRow = new ProjectSize
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
