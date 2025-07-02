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
    public class StateRepository : DataBaseRepository, IStateRepository
    {
        public StateRepository()
        {
            TABLE_NAME = "State";
        }

        public State Get(long id)
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R where R.Id = '{1}' ", TABLE_NAME, id);

            var result = ExecuteQuery(query);
            var row = result[0];
            return new State
            {
                Id = ToLong(row["Id"]),
                Code = ToString(row["Code"]),
                Descripcion = ToString(row["Description"])
            };
        }

        public IEnumerable<State> GetAll()
        {
            string query = string.Format("SELECT R.Id, R.Code, R.Description FROM {0} R ", TABLE_NAME);

            var result = ExecuteQuery(query);

            List<State> entities = new List<State>();

            foreach (DataRow row in result)
            {
                entities.Add(new State
                {
                    Id = ToLong(row["Id"]),
                    Code = ToString(row["Code"]),
                    Descripcion = ToString(row["Description"])
                });
            }

            return entities;
        }
    }
}
