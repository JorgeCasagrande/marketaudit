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
    public class RouteRepository : DataBaseRepository, IRouteRepository
    {
        public RouteRepository()
        {
            TABLE_NAME = "ROUTE";
        }

        public IEnumerable<RouteModel> GetRouteByUserIdAndProjectId(long userId, long projectId)
        {
            string query = string.Format("SELECT R.Id, R.ProjectId, R.NAME AS RouteName, R.Description AS RouteDescription, R.Image as RouteImage " +
                "FROM {0} R " +
                "WHERE R.CensistId = {1} AND R.ProjectId = {2}", TABLE_NAME, userId, projectId);

            var result = ExecuteQuery(query);

            List<RouteModel> entities = new List<RouteModel>();

            foreach (DataRow row in result)
            {

                RouteModel itemRow = new RouteModel();

                itemRow.Id = ToLong(row["Id"]);
                itemRow.projectId = ToLong(row["ProjectId"]);
                itemRow.RouteName = ToString(row["RouteName"]);
                itemRow.RouteDescription = ToString(row["RouteDescription"]);
                itemRow.Map = ToString(row["RouteImage"]);

                entities.Add(itemRow);
            }
            
            return entities;
        }

        public IEnumerable<RouteModel> GetRouteByUserId(long userId)
        {
            string query = string.Format("SELECT R.Id, R.ProjectId, R.NAME AS RouteName, R.Description AS RouteDescription, R.Image as RouteImage " +
                "FROM {0} R " +
                "WHERE R.CensistId = {1}", TABLE_NAME, userId);

            var result = ExecuteQuery(query);

            List<RouteModel> entities = new List<RouteModel>();

            foreach (DataRow row in result)
            {

                RouteModel itemRow = new RouteModel();

                itemRow.Id = ToLong(row["Id"]);
                itemRow.projectId = ToLong(row["ProjectId"]);
                itemRow.RouteName = ToString(row["RouteName"]);
                itemRow.RouteDescription = ToString(row["RouteDescription"]);
                itemRow.Map = ToString(row["RouteImage"]);

                entities.Add(itemRow);
            }

            return entities;
        }

        public long GetRouteId(long projectId, long userId, long pdvId)
        {
            string query = string.Format("SELECT R.ID FROM {0} R " +
                "JOIN Routes_Pdvs RP on R.Id = RP.RouteId WHERE R.ProjectId = {1} AND R.CensistId = {2} AND RP.PdvId = {3}",
                TABLE_NAME, projectId, userId, pdvId);

            var result = ExecuteQuery(query);

            var row = result[0];

            return ToLong(row["ID"]);
        }

        public long GetRoute(long projectId)
        {
            string query = string.Format("SELECT count(R.ID) as ID FROM {0} R JOIN Routes_Pdvs RP on R.Id = RP.RouteId WHERE R.ProjectId = {1}", TABLE_NAME, projectId);

            var result = ExecuteQuery(query);

            var row = result[0];

            return ToLong(row["ID"]);

        }

        public Route Get(long projectId, long censistId, string name, TransactionalContext transaction)
        {
            string query = string.Format("SELECT R.Id, R.Name, R.Description, " +
                "R.ProjectId, R.CensistId FROM {0} R " +
                "where R.Name = '{1}' and R.ProjectId = '{2}' " +
                "and R.CensistId = '{3}' ", TABLE_NAME, name, projectId, censistId);

            var result = ExecuteQuery(query, transaction);
            var row = result[0];
            return new Route
            {
                Id = ToLong(row["Id"]),
                Name = ToString(row["Name"]),
                Description = ToString(row["Description"]),
                ProjectId = ToLong(row["ProjectId"]),
                CensistId = ToLong(row["CensistId"])
            };
        }

        public bool Exist(long projectId, long censistId, string name, TransactionalContext transaction)
        {
            string query = string.Format("SELECT count(R.Id) as ID FROM {0} R where R.Name = '{1}' and R.ProjectId = '{2}' and R.CensistId = '{3}' ", 
                TABLE_NAME, name, projectId, censistId);

            var result = ExecuteQuery(query, transaction);
            var row = result[0];
            return ToLong(row["ID"]) > 0;
        }

        public long Insert(Route model, TransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [{0}]  ([Name], [Description], [ProjectId], [CensistId]) " +
              " VALUES ('{1}','{2}','{3}','{4}') select scope_identity() as id",
              TABLE_NAME, RemoveCharacterInvalid(model.Name), RemoveCharacterInvalid(model.Description), model.ProjectId, model.CensistId);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToLong(row["id"]);
        }

        public void InsertRoutePdv(RoutePdv model, TransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [Routes_Pdvs]  ([RouteId], [PdvId]) " +
              " VALUES ('{0}','{1}')", model.RouteId, model.PdvId);

            ExecuteQuery(query, transaction);
        }

        public IEnumerable<Filter> GetRouteFilter(long projectId, TransactionalContext transaction)
        {
            string query = string.Format("SELECT R.Id, CONCAT(R.NAME, '-' , U.USERNAME) AS RouteName " +
                "FROM {0} R " +
                "join [User] u on r.CensistId = u.Id " +
                "WHERE R.ProjectId = {1}", TABLE_NAME, projectId);

            var result = ExecuteQuery(query);

            List<Filter> entities = new List<Filter>();

            foreach (DataRow row in result)
            {
                Filter itemRow = new Filter();

                itemRow.Key = ToLong(row["Id"]);
                itemRow.Value = ToString(row["RouteName"]);

                entities.Add(itemRow);
            }

            return entities;
        }

        public void DeleteRoutesProject(long projectId, ITransactionalContext transaction)
        {
            string query = string.Format("EXEC dbo.DeleteRouteAndPdv @ProjectId = {1}", TABLE_NAME, projectId);

            ExecuteQuery(query, transaction);
        }
    }
}
