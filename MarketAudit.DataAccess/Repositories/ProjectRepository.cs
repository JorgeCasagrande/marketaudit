using Marketaudit.Entities.Models.Response;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using MarketAudit.Entities;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models.Request;

namespace Marketaudit.DataAccess.Repositories
{
    public class ProjectRepository : DataBaseRepository, IProjectRepository
    {
        public ProjectRepository()
        {
            TABLE_NAME = "PROJECT";
        }

        public void Delete(long id, ITransactionalContext transaction)
        {
            string query = string.Format("EXEC dbo.DeleteProject @ProjectIdDelete = {1}", TABLE_NAME, id);

            ExecuteQuery(query, transaction);
        }

        public void Enable(long id, long state, ITransactionalContext transaction)
        {
            string query = string.Format("UPDATE {0} SET [StateId] = '{1}' WHERE Id = {2}", TABLE_NAME, state, id);

            ExecuteQuery(query, transaction);
        }

        public Project Get(long id)
        {
            string whereClause = string.Format("Where c.Id ={0}", id);
            string query = string.Format("SELECT c.Id, c.Name, c.Description, c.ProjectTypeId, c.CustomerId, " +
                " c.ResponsableId, c.SAS, c.Creation, c.StartDate, c.FinishDate, c.StateId FROM {0} c {1} ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query);

            var row = result[0];

            return new Project
            {
                Id = ToLong(row["Id"]),
                Name = ToString(row["Name"]),
                Description = ToString(row["Description"]),
                ProjectTypeId = !string.IsNullOrEmpty(ToString(row["ProjectTypeId"])) ? ToLong(row["ProjectTypeId"]) : 0,
                CustomerId = !string.IsNullOrEmpty(ToString(row["CustomerId"])) ? ToLong(row["CustomerId"]) : 0,
                ResponsableId = !string.IsNullOrEmpty(ToString(row["ResponsableId"])) ? ToLong(row["ResponsableId"]) : 0,
                Sas = !string.IsNullOrEmpty(ToString(row["SAS"])) ? Convert.ToBoolean(ToString(row["SAS"])) : false,
                StateId = !string.IsNullOrEmpty(ToString(row["StateId"])) ? ToLong(row["StateId"]) : 0,
                Creation = Convert.ToDateTime(ToString(row["Creation"])),
                StartDate = Convert.ToDateTime(ToString(row["StartDate"])),
                FinishDate = Convert.ToDateTime(ToString(row["FinishDate"]))
            };
        }

        public IEnumerable<Project> GetProjects(string states)
        {
            string whereClause = string.Empty;
            if (!string.IsNullOrEmpty(states) && states.Length > 0)
            {
                whereClause = string.Format("Where c.StateId in ({0})", states);
            }
            string query = string.Format("SELECT c.Id, c.Name, c.Description, c.ProjectTypeId, c.CustomerId, " +
                " c.ResponsableId, c.SAS, c.Creation, c.StartDate, c.FinishDate FROM {0} c {1} Order by c.Name asc ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query);

            List<Project> entities = new List<Project>();

            foreach (DataRow row in result)
            {

                Project itemRow = new Project
                {
                    Id = ToLong(row["Id"]),
                    Name = ToString(row["Name"]),
                    Description = ToString(row["Description"]),
                    ProjectTypeId = !string.IsNullOrEmpty(ToString(row["ProjectTypeId"])) ? ToLong(row["ProjectTypeId"]) : 0,
                    CustomerId = !string.IsNullOrEmpty(ToString(row["CustomerId"])) ? ToLong(row["CustomerId"]) : 0,
                    ResponsableId = !string.IsNullOrEmpty(ToString(row["ResponsableId"])) ? ToLong(row["ResponsableId"]) : 0,
                    Sas = !string.IsNullOrEmpty(ToString(row["SAS"])) ? Convert.ToBoolean(ToString(row["SAS"])) : false,
                    StateId = !string.IsNullOrEmpty(ToString(row["StateId"])) ? ToLong(row["StateId"]) : 0,
                    Creation = Convert.ToDateTime(ToString(row["Creation"])),
                    StartDate = Convert.ToDateTime(ToString(row["StartDate"])),
                    FinishDate = Convert.ToDateTime(ToString(row["FinishDate"]))
                };

                entities.Add(itemRow);
            }

            return entities;
        }

        public IEnumerable<ProjectGrid> GetProjectsByGrid(string states, string responsables)
        {
            string whereClause = "Where 1 = 1 ";
            if (!string.IsNullOrEmpty(states) && states.Length > 0)
            {
                whereClause += string.Format(" and c.StateId in ({0})", states);
            }
            if (!string.IsNullOrEmpty(responsables) && responsables.Length > 0)
            {
                whereClause += string.Format(" and c.ResponsableId in ({0})", responsables);
            }

            string query = string.Format("SELECT c.Id, c.[Name], c.[Description], type.[Description] as ProjectType, customer.[Name] as Customer, " +
                "u.[Name] as [Responsable], s.[Description] as State, c.SAS, c.Creation, c.StartDate, c.FinishDate, " +
                "(select count(pdvs.PdvId) from[route] r join[routes_Pdvs] pdvs on r.Id = pdvs.RouteId and r.ProjectId = c.Id) as CantidadPdv, " +
                "(select count(r.CensistId) from[route] r where r.ProjectId = c.Id) as CantidadCensistas, " +
                "(select count(pq.Id) from[Project_Questions] pq where pq.ProjectId = c.Id) as CantidadPreguntas " +
                "FROM {0} c " +
                "join Project_Type type on type.Id = c.ProjectTypeId " +
                "join Customer customer on customer.Id = c.CustomerId " +
                "join[user] u on u.Id = c.ResponsableId " +
                "join[state] s on s.Id = c.StateId " +
                "{1} Order by c.Name asc  ", TABLE_NAME, whereClause);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();
            List<ProjectGrid> entities;

            using (conn)
            {
                entities = conn.Query<ProjectGrid>(query).ToList();
                conn.Close();
            }

            return entities;
        }

        public IEnumerable<ProjectGrid> GetProjectsReport()
        {
            string query = string.Format("SELECT c.Id, c.[Name] " +
                " FROM {0} c " +
                " Order by c.Name ", TABLE_NAME);


            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();
            List<ProjectGrid> entities;

            using (conn)
            {
                entities = conn.Query<ProjectGrid>(query).ToList();
                conn.Close();
            }

            return entities;
        }

        public IEnumerable<ProjectsModel> GetProjectsByUserId(long userId)
        {
            string query = string.Format("SELECT distinct P.Id, P.Name, C.Name as Client, PT.Description AS ProjectType, S.Description AS ProjectStatus " +
                "FROM Route R " +
                "JOIN {0} P ON R.ProjectId = P.Id " +
                "JOIN Customer C on P.CustomerId = C.Id " +
                "JOIN Project_Type PT ON P.ProjectTypeId = PT.Id " +
                "JOIN State S ON P.StateId = S.ID " +
                "LEFT JOIN Routes_Pdvs RP ON RP.RouteId = r.Id " +
                "JOIN [User] U ON U.Id = R.CensistId " +
                "WHERE R.CensistId = {1}  " +
                "AND ((S.CODE = 'APP' AND U.IsUserTest = 0) " +
                "OR (S.CODE IN('CREATE') AND U.IsUserTest = 1)) "
                , TABLE_NAME, userId);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();
            List<ProjectsModel> entities;

            using (conn)
            {
                entities = conn.Query<ProjectsModel>(query).ToList();
                conn.Close();
            }

            return entities;
        }

        public void Insert(Project model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO {0} ([Name], [Description], [ProjectTypeId], [CustomerId], [ResponsableId], " +
                "[SAS], [StateId], [Creation], [StartDate], [FinishDate]) " +
                "VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                TABLE_NAME, RemoveCharacterInvalid(model.Name), RemoveCharacterInvalid(model.Description), model.ProjectTypeId, model.CustomerId, model.ResponsableId,
                model.Sas, model.StateId, model.Creation, model.StartDate, model.FinishDate);
            
            ExecuteQuery(query, transaction);

        }

        public void Update(Project model, ITransactionalContext transaction)
        {
            string query = string.Format("UPDATE {0} SET [Name] = '{1}', [Description] = '{2}', [ProjectTypeId] = '{3}'," +
                "[CustomerId] = '{4}', [ResponsableId] = '{5}', [SAS] = '{6}'," +
                "[StateId] = '{7}', [StartDate] = '{8}', [FinishDate] = '{9}' WHERE Id = {10}",
                TABLE_NAME, RemoveCharacterInvalid(model.Name), RemoveCharacterInvalid(model.Description), model.ProjectTypeId, model.CustomerId, model.ResponsableId,
                model.Sas, model.StateId, model.StartDate, model.FinishDate, model.Id);

            ExecuteQuery(query, transaction);
        }

        public void InsertProjectQuestion(ProjectQuestion model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [Project_Questions]  ([ProjectId],[QuestionId], [Orden]) " +
                " VALUES ('{0}','{1}','{2}')", model.ProjectId, model.QuestionId, model.Order);

            ExecuteQuery(query, transaction);
        }

        public void FixDuplicateRoutes(long projectId, ITransactionalContext transactional)
        {
            string query = string.Format("FixDuplicateRoutes");
            List<Parametros> parametros = new List<Parametros>();
            parametros.Add(new Parametros { Parametro = "@projectIdToUpdate", Valor = projectId });

            ExecuteStoreProcedure(query, parametros, transactional);
        }

        public void CreateReportProjectTable(long projectId, List<ImportQuestionModel> questions)
        {
            var tableName = "Project_" + projectId;
            var staticColumns = "Usuario VARCHAR(50), Codigo_PDV VARCHAR(50), PDV VARCHAR(50), Ruta VARCHAR(20), Fecha VARCHAR(20), Hora VARCHAR(20), ";
            var columns = staticColumns + string.Join(',', questions.Select(q => "F_"+q.Orden + (q.TipoPregunta == "IMG" ? " VARCHAR(MAX)" : " VARCHAR(255)")));

            string query = "CREATE TABLE " + tableName + " (" + columns + ") ";

            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();

            using (conn)
            {
                conn.Query(query);
                conn.Close();
            }

        }

        public void CreateReportProjectTable(long projectId, List<QuestionModel> questions)
        {
            var tableName = "Project_" + projectId;
            var staticColumns = "Usuario VARCHAR(255), Codigo_PDV VARCHAR(255), PDV VARCHAR(255), Ruta VARCHAR(255), Fecha VARCHAR(255), Hora VARCHAR(255), ";
            var columns = staticColumns + string.Join(',', questions.Select(q => "F_" + q.Order + (q.QuestionImg == "IMG" ? " VARCHAR(MAX)" : " VARCHAR(MAX)")));

            string query = "CREATE TABLE " + tableName + " (" + columns + ") ";

            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();

            using (conn)
            {
                conn.Query(query);
                conn.Close();
            }

        }

        public void DropReportProjectTable(long projectId)
        {
            var tableName = "Project_" + projectId;

            string query = string.Format("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{0}') " +
                "BEGIN " +
                "DROP TABLE {0} " +
                "END", tableName);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();

            using (conn)
            {
                conn.Query(query);
                conn.Close();
            }

        }

        public bool ExistsReportProjectTable(long projectId, string usuario, string codigoPdv, string pdv, string ruta)
        {
            var tableName = "Project_" + projectId;

            string query = string.Format("IF EXISTS (SELECT * FROM {0} where Usuario = '{1}' and Codigo_Pdv = '{2}' and PDV = '{3}' and Ruta = '{4}') " +
                "BEGIN " +
                "SELECT 1 " +
                "END " +
                "ELSE " +
                "BEGIN " +
                "SELECT 0 " +
                "END", tableName, usuario, codigoPdv, pdv, ruta);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();

            bool result;

            using (conn)
            {
                result = conn.QueryFirst<bool>(query);
                conn.Close();
            }

            return result;
        }

        public void InsertReport(long projectId, string usuario, string codigoPdv, string pdv, string ruta, string fecha, string hora)
        {
            var tableName = "Project_" + projectId;
            var query = string.Format("INSERT INTO {0} (Usuario, Codigo_Pdv, PDV, Ruta, Fecha, Hora) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}')", tableName, usuario, codigoPdv, pdv, ruta, fecha, hora);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();

            using (conn)
            {
                conn.Query(query);
                conn.Close();
            }
        }

        public void UpdateReport(long projectId, string usuario, string codigoPdv, string pdv, string ruta, string value, string order)
        {
            var tableName = "Project_" + projectId;
            var query = string.Format("UPDATE {0} SET F_{5} = '{6}' WHERE Usuario = '{1}' and Codigo_Pdv = '{2}' and PDV = '{3}' and Ruta = '{4}' ", tableName, usuario, codigoPdv, pdv, ruta, order, (!string.IsNullOrEmpty(value) ? value : "-"));

            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();

            using (conn)
            {
                conn.Query(query);
                conn.Close();
            }
        }

        public IEnumerable<object> GetReport(long projectId)
        {
            var tableName = "Project_" + projectId;
            var query = string.Format("SELECT *FROM {0}", tableName);
            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();
            List<object> result;
            using (conn)
            {
                result = conn.Query<object>(query).ToList();
                conn.Close();
            }

            return result;
        }
        public bool ExistsTableReport(long projectId)
        {
            string query = string.Format("IF (EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Project_{0}')) " +
                "BEGIN SELECT 1 AS Result END " +
                "ELSE BEGIN SELECT 0 AS Result END ", projectId);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetReportDatabaseConnectionString());
            conn.Open();
            bool result = false;

            using (conn)
            {
                result = conn.QueryFirst<bool>(query);
                conn.Close();
            }

            return result;
        }

        public void DeleteDuplicatePdvs(long projectId, ITransactionalContext transaction)
        {
            string query = string.Format("EXEC dbo.DeleteDuplicatePdvs @ProjectId = {1}", TABLE_NAME, projectId);

            ExecuteQuery(query, transaction);
        }
    }
}
