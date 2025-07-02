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
    public class RolRepository : DataBaseRepository, IRolRepository
    {
        public RolRepository()
        {
            TABLE_NAME = "Role";
        }

        public Rol Get(long id)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.Id = '{1}' ", TABLE_NAME, id);

            var result = ExecuteQuery(query);
            var row = result[0];
            return new Rol
            {
                Id = ToLong(row["Id"]),
                Code = ToString(row["Code"]),
                Descripcion = ToString(row["Description"])
            };
        }

        public IEnumerable<Rol> GetRoles()
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R ", TABLE_NAME);

            var result = ExecuteQuery(query);

            List<Rol> entities = new List<Rol>();

            foreach (DataRow row in result)
            {
                Rol itemRow = new Rol();

                itemRow.Id = ToLong(row["Id"]);
                itemRow.Code = ToString(row["Code"]);
                itemRow.Descripcion = ToString(row["Description"]);
                
                entities.Add(itemRow);
            }

            return entities;
        }
    }
}
