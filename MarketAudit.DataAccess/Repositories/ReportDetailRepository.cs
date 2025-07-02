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
    public class ReportDetailRepository : DataBaseRepository, IReportDetailRepository
    {
        public ReportDetailRepository()
        {
            TABLE_NAME = "Report_Detail";
        }

        public List<ReportDetail> GetReportDetailsByProject(long projectId, ITransactionalContext context)
        {
            string query = string.Format("select u.Id as UserId,u.UserName, " +
                    " pdv.Number as PdvNumber, " +
                    " pdv.Name as PdvName, " +
                    " route.Name as Route, " +
                    " rm.Id as ReportMasterId, " +
                    " rm.Creation as Creation, " +
                    "rd.* from {0} rd" +
                    " join Report_Master rm on rd.ReportMasterId = rm.Id" +
                    " join [User] u on rm.UserId = u.Id " +
                    " join [Pdv] pdv on rm.PdvId = pdv.Id " +
                    " join [Route] route on rm.RouteId = route.Id " +
                    "where rm.ProjectId= {1}", TABLE_NAME, projectId);


            var result = ExecuteQuery(query, context);

            List<ReportDetail> detail = new List<ReportDetail>();

            foreach (DataRow row  in result)
            {
                ReportDetail item = new ReportDetail();
                item.Id = ToLong(row["Id"]);
                item.ReportMasterId = ToLong(row["ReportMasterId"]);
                item.QuestionId = ToLong(row["QuestionId"]);
                item.Value = ToString(row["Value"]);
                item.Order = ToInt(row["Order"]);
                item.ReportMaster = new ReportMaster();
                item.ReportMaster.User = new User();
                item.ReportMaster.User.Id = ToLong(row["UserId"]);
                item.ReportMaster.User.UserName = ToString(row["UserName"]);
                item.ReportMaster.Creation = ToDateTime(row["Creation"]);
                item.ReportMaster.Pdv = new PdvEntity();
                item.ReportMaster.Pdv.Name = ToString(row["PdvName"]);
                item.ReportMaster.Pdv.Number = ToLong(row["PdvNumber"]);
                item.ReportMaster.Route = new Route();
                item.ReportMaster.Route.Name = ToString(row["Route"]);
                item.ReportMaster.Id = ToLong(row["Id"]);
                detail.Add(item);

            }

            return detail;
        }

        public List<PhotosReport> GetReportPhotosByProject(long projectId, string users , string pdvs, string routes, string questions ,ITransactionalContext context)
        {
            string query = string.Format("select rd.Id as Id,u.UserName," +
                "pdv.Number as PdvNumber," +
                "pdv.Name as PdvName," +
                "route.Name as RouteName," +
                "q.Question as Question," +
                "rd.Value from {0} rd " +
                "join Report_Master rm on rd.ReportMasterId = rm.Id " +
                "join [User] u on rm.UserId = u.Id " +
                "join [Pdv] pdv on rm.PdvId = pdv.Id " +
                "join [Route] route on rm.RouteId = route.Id " +
                "join [Question] q on rd.QuestionId = q.Id " +
                "where rm.ProjectId = {1} ", TABLE_NAME, projectId);

            if(!string.IsNullOrEmpty(users))
            {
                query += string.Format("and rm.UserId in ({0}) ", users);
            }

            if (!string.IsNullOrEmpty(pdvs))
            {
                query += string.Format("and pdv.number in ({0}) ", pdvs);
            }

            if (!string.IsNullOrEmpty(routes))
            {
                query += string.Format("and rm.RouteId in ({0}) ", routes);
            }

            if (!string.IsNullOrEmpty(questions))
            {
                query += string.Format("and rd.QuestionId in ({0}) ", questions);
            }

            query += "and rd.[Value] like 'https://weask-images.s3.amazonaws.com%'";

            var result = ExecuteQuery(query, context);

            List<PhotosReport> detail = new List<PhotosReport>();

            foreach (DataRow row in result)
            {
                var img = ToString(row["Value"]);

                if (!img.Contains("Fstorage"))
                {
                    if (img.Any(q => q.ToString().Contains('|')))
                    {
                        int c = 1;
                        foreach (var i in img.Split('|'))
                        {
                            PhotosReport item = new PhotosReport();
                            item.Id = ToLong(row["Id"]).ToString() + '(' + c + ')';
                            item.Img = i;
                            item.Title = ToString(row["PdvNumber"]) + " - " + ToString(row["PdvName"]) + " - " + ToString(row["RouteName"]); ;
                            item.Author = ToString(row["UserName"]);
                            item.Question = ToString(row["Question"]);
                            detail.Add(item);
                            c++;
                        }
                    }
                    else
                    {
                        PhotosReport item = new PhotosReport();
                        item.Id = ToLong(row["Id"]).ToString();
                        item.Img = img;
                        item.Title = ToString(row["PdvNumber"]) + " - " + ToString(row["PdvName"]) + " - " + ToString(row["RouteName"]); ;
                        item.Author = ToString(row["UserName"]);
                        item.Question = ToString(row["Question"]);
                        detail.Add(item);
                    }
                }
            }

            return detail;

        }

        public FilterPhotos GetFilterPhotosByProject(long projectId, ITransactionalContext context)
        {
            string query = string.Format("select rm.Id," +
                "u.Id as UserId,u.UserName," +
                "pdv.Id as PdvId," +
                "pdv.Number as PdvNumber," +
                "pdv.Name as PdvName " +
                "from Report_Master rm " +
                "join [User] u on rm.UserId = u.Id " +
                "join [Pdv] pdv on rm.PdvId = pdv.Id " +
                "join [Route] route on rm.RouteId = route.Id " +
                "where rm.ProjectId = {1} ", TABLE_NAME, projectId);


            var result = ExecuteQuery(query, context);


            FilterPhotos detail = new FilterPhotos();
            detail.Users = new List<Filter>();
            detail.Pdvs = new List<Filter>();
            
            foreach (DataRow row in result)
            {
                var user = new Filter();
                var pdv = new Filter();

                user.Key = ToLong(row["UserId"]);
                user.Value = ToString(row["UserName"]);
                pdv.Key = ToLong(row["PdvId"]);
                pdv.Value = ToString(row["PdvNumber"]) + " - " + ToString(row["PdvName"]);

                detail.Users.Add(user);
                detail.Pdvs.Add(pdv);
            }

            return detail;

        }

        public List<ReportModel> GetReportDetailsByProjectId(long projectId)
        {
            string query = string.Format("select rm.Id as ReportMasterId, rm.UserName, " +
                "rm.PdvNumber as PdvNumber, " +
                "rm.Pdv as PdvName, rm.[Route] as Route, rm.Creation as Creation, rd.Value, rd.[Order] " +
                "from Report_Master rm " +
                "join Report_Detail rd on rd.ReportMasterId = rm.Id " +
                "where rm.ProjectId = {0}"
                , projectId);

            if (projectId == 52416)
            {
                var desde = DateTime.Today.AddDays(-7).Year.ToString("0000") + DateTime.Today.AddDays(-7).Month.ToString("00") + DateTime.Today.AddDays(-7).Day.ToString("00");
                var hasta = DateTime.Today.Year.ToString("0000") + DateTime.Today.Month.ToString("00") + DateTime.Today.Day.ToString("00");

                query += string.Format(" and rm.Creation between '{0}' and '{1}' ", desde, hasta);
            }

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();
            
            List<ReportModel> entities;

            using (conn)
            {
                entities = conn.Query<ReportModel>(query).ToList();
                conn.Close();
            }

            return entities;
        }

        public void DeleteReport(long projectId, ITransactionalContext transaction)
        {
            string query = string.Format("EXEC dbo.DeleteReport @ProjectId = {1}", TABLE_NAME, projectId);

            ExecuteQuery(query, transaction);
        }
    }
}
