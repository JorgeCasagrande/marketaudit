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

namespace Marketaudit.DataAccess.Repositories
{
    public class PdvRepository : DataBaseRepository, IPdvRepository
    {
        public PdvRepository()
        {
            TABLE_NAME = "PDV";
        }

        public IEnumerable<PdvModel> GetPdvByRouteId(long routeId)
        {
            string query = string.Format("SELECT P.Id, RP.RouteId, P.Number AS PdvNumber, P.Name AS PdvName, P.Description as PdvDescription, P.Address AS PdvAddress " +
                "FROM Routes_Pdvs RP " +
                "JOIN {0} P ON RP.PdvId = P.Id " +
                "WHERE RP.RouteId = {1}", TABLE_NAME, routeId);

            var result = ExecuteQuery(query);

            List<PdvModel> entities = new List<PdvModel>();

            foreach (DataRow row in result)
            {
                PdvModel itemRow = new PdvModel();

                itemRow.Id = ToLong(row["Id"]);
                itemRow.RouteId = ToLong(row["RouteId"]);
                itemRow.PdvNumber = ToLong(row["PdvNumber"]);
                itemRow.PdvName = ToString(row["PdvName"]);
                itemRow.PdvDescription = ToString(row["PdvDescription"]);
                itemRow.Address = ToString(row["PdvAddress"]);

                entities.Add(itemRow);
            }

            return entities;
        }

        public int GetCountPdvByProjectIdAndUserId(long projectId, long userId)
        {
            string query = string.Format("SELECT COUNT(RP.ID) CountPdv FROM Routes_Pdvs RP " +
                "JOIN ROUTE R ON RP.RouteId = R.Id " +
                "WHERE R.ProjectId = {0} AND R.CensistId = {1}", projectId, userId);

            var result = ExecuteQuery(query);

            DataRow entity = result[0];

            return ToInt(entity["CountPdv"]);
        }

        public IEnumerable<PdvModel> GetPdvByUserId(long userId)
        {
            string query = string.Format("SELECT P.Id, RP.RouteId, P.Number AS PdvNumber, P.Name AS PdvName, P.Description as PdvDescription, P.Address AS Address " +
                " , Case when Exists (Select * from report_master where pdvId = P.Id) then 1 else 0 end as Completed "+
                "FROM Routes_Pdvs RP " +
                "JOIN {0} P ON RP.PdvId = P.Id " +
                "JOIN Route R ON RP.RouteId = R.Id " +
                "WHERE R.CensistId = {1} AND P.VISIBLE = 1 ", TABLE_NAME, userId);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();
            List<PdvModel> entities;

            using (conn)
            {
                entities = conn.Query<PdvModel>(query).ToList();
                conn.Close();
            }

            return entities;
        }

        public IEnumerable<PdvGridProject> GetPdvByProyectId(long projectId)
        {
            string query = string.Format("select p.Name as PdvName, p.Description as PdvDescription, p.Number as PdvNumber, " +
                "p.Cuit as PdvCuit, p.Address PdvAddress, ptype.Description as PdvType, p.Notes as PdvNotes, " +
                "r.Name as RouteName, r.Description as RouteDescription, u.UserName as Censist " +
                "from Routes_Pdvs rp inner join [Route] r on rp.RouteId = r.Id " +
                "left join [User] u on r.CensistId = u.Id join Pdv p on rp.PdvId = p.Id " +
                "join Pdv_Type ptype on p.PdvTypeId = ptype.Id " +
                "WHERE R.ProjectId = {0}", projectId);

            var result = ExecuteQuery(query);

            List<PdvGridProject> entities = new List<PdvGridProject>();

            foreach (DataRow row in result)
            {
                PdvGridProject itemRow = new PdvGridProject();
                itemRow.Number = ToLong(row["PdvNumber"]);
                itemRow.Name = ToString(row["PdvName"]);
                itemRow.Description = ToString(row["PdvDescription"]);
                itemRow.Description = ToString(row["PdvDescription"]);
                itemRow.Cuit = ToString(row["PdvCuit"]);
                itemRow.Address = ToString(row["PdvAddress"]);
                itemRow.Type = ToString(row["PdvType"]);
                itemRow.Notes = ToString(row["PdvNotes"]);
                itemRow.RouteName = ToString(row["RouteName"]);
                itemRow.RouteDescription = ToString(row["RouteDescription"]);
                itemRow.Censist = ToString(row["Censist"]);

                entities.Add(itemRow);
            }

            return entities;
        }

        public long Insert(PdvEntity model, TransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [{0}]  ([Name], [Number], [Description], [Cuit], [Address], [PdvTypeId], [Notes]) " +
               " VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}') select scope_identity() as id",
               TABLE_NAME, RemoveCharacterInvalid(model.Name), model.Number, RemoveCharacterInvalid(model.Description), RemoveCharacterInvalid(model.Cuit), RemoveCharacterInvalid(model.Address), model.PdvTypeId, RemoveCharacterInvalid(model.Notes));

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToLong(row["id"]);
        }

        public long InsertSingle(PdvEntity model, TransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [{0}]  ([Name], [Number], [Description], [Cuit], [Address], [PdvTypeId], [Notes],[Visible]) " +
               " VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}') select scope_identity() as id",
               TABLE_NAME, RemoveCharacterInvalid(model.Name), model.Number, RemoveCharacterInvalid(model.Description), RemoveCharacterInvalid(model.Cuit), RemoveCharacterInvalid(model.Address), model.PdvTypeId, RemoveCharacterInvalid(model.Notes), (model.Visible == "Si"? 1: 0));

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToLong(row["id"]);
        }

        public IEnumerable<Filter> GetPdvFilter(long projectId, string userId, TransactionalContext transaction)
        {
            string query = @"Select Distinct P.Number as [Key], CONCAT(P.Name, '-', P.Number) as Value 
                     from Route R 
                     Join Routes_Pdvs rp on R.Id = rp.RouteId 
                     join Pdv p on RP.PdvId = P.Id 
                     where R.ProjectId = @ProjectId " + 
                     (!string.IsNullOrEmpty(userId) ? "and r.censistId in (@userId) " : string.Empty) +
                     "ORDER BY P.NUMBER";



            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();

            List<Filter> entities;

            using (conn)
            {
                entities = conn.Query<Filter>(query, new { ProjectId = projectId, userId = userId }).ToList();
                conn.Close();
            }

            return entities;
        }

        public IEnumerable<ProjectPdvTemplateModel> GetPdvTemplateByProjectId(long projectId)
        {
            string query = @"
                Select 
                p.Id,
                u.UserName as Censista,
                p.Number as Numero,
                p.Name as Nombre,
                p.Cuit,
                p.Address as Direccion,
                pt.[Description] as Tipo,
                r.Name as Ruta,
                CASE 
                    WHEN p.Visible = 1 THEN 'Si'
                    ELSE 'No' 
                    END as Visible
                from [Route] r
                join Routes_Pdvs rp on r.Id = rp.RouteId
                join [User] u on r.CensistId = u.Id
                join Pdv p on rp.PdvId = p.Id
                join Pdv_Type pt on p.PdvTypeId = pt.Id
                WHERE r.ProjectId = @ProjectId";

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();

            List<ProjectPdvTemplateModel> entities;

            using (conn)
            {
                entities = conn.Query<ProjectPdvTemplateModel>(query, new { ProjectId = projectId }).ToList();
                conn.Close();
            }

            return entities;
        }
    }
}
