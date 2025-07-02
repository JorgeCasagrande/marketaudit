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
    public class ReportRepository : DataBaseRepository, IReportRepository
    {
        public ReportRepository()
        {
            TABLE_NAME = "Report_Master";
        }

        public void Create(long userId, long projectId, long pdvId, long routeId, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO {0} ([ProjectId],[UserId],[PdvId],[RouteId],[Creation],[StateId]) VALUES " +
                "({1},{2},{3},{4},(SELECT GETDATE()),(SELECT ID FROM State_Report WHERE CODE = 'PENDING'))", TABLE_NAME, projectId, userId, pdvId, routeId);

            ExecuteQuery(query, transaction);
        }

        public long GetReportMasterId(long userId, long projectId, long pdvId, long routeId, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT ID FROM {0} WHERE ProjectId = {1} AND UserId = {2} AND PdvId = {3} AND RouteId = {4}", TABLE_NAME, projectId, userId, pdvId, routeId);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToLong(row["ID"]);
        }

        public int GetOrderQuestion(long questionId, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT Orden from Project_questions where Id = {0}", questionId);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToInt(row["Orden"]);
        }

        public void CreateDetail(long reportMasterId, long projectId, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO Report_Detail (ReportMasterId, QuestionId, [Value], [Order], StateId) " +
                "(SELECT {0}, QuestionId, null, Orden, (SELECT ID FROM State_Report WHERE CODE = 'PENDING') FROM Project_Questions Where ProjectId = {1})", reportMasterId, projectId);

            ExecuteQuery(query, transaction);
        }

        public void UpdateDetail(long reportMasterId, long questionId, int order, string Value, ITransactionalContext transaction)
        {
            string query = string.Format("UPDATE Report_Detail SET [Value] = '{0}' WHERE ReportMasterId = {1} AND [Order] = {2}", RemoveCharacterInvalid(Value), reportMasterId, order);

            ExecuteQuery(query, transaction);
        }

        public bool ExistReport(long userId, long projectId, long pdvId, long routeId, ITransactionalContext transaction)
        {
            string query = string.Format("SELECT COUNT(ID) AS CONTADOR FROM {0} WHERE ProjectId = {1} AND UserId = {2} AND PdvId = {3} AND RouteId = {4}", TABLE_NAME, projectId, userId, pdvId, routeId);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return ToInt(row["CONTADOR"]) > 0;
        }

        public ReportPdv GetReportPdv(long projectId, long userId)
        {
            string query = string.Format("ReportResumenPdvByUser");
            List<Parametros> parametros = new List<Parametros>();
            parametros.Add(new Parametros { Parametro = "@projectId", Valor = projectId });
            parametros.Add(new Parametros { Parametro = "@userId", Valor = userId });

            var result = ExecuteStoreProcedure(query, parametros);

            var row = result[0];

            ReportPdv entity = new ReportPdv();

            entity.PdvPendientes = ToInt(row["CantidadPdvs"]) - ToInt(row["PdvEnviados"]);
            entity.AvanceCampo = this.ValidateDivision((ToDecimal(row["PdvEnviados"]) * 100) , ToDecimal(row["CantidadPdvs"]));
            entity.PromedioDiario = this.ValidateDivision(ToDecimal(row["CantidadPdvs"]) , ToDecimal(row["CantidadDias"]));
            entity.Tendencia = decimal.Round(5,2);
            entity.LastSendMessage = this.SetMesageLastPdv(ToNullableDateTime(row["LastSendPdv"]));
            entity.ConsumedTime = this.ValidateDivision((ToDecimal(row["ConsumedTime"]) * 100) , ToDecimal(row["TotalTime"]));

            return entity;
        }

        private decimal ValidateDivision(decimal n1, decimal n2)
        {
            decimal result = 0;
            if(n2 > 0)
            {
                result = n1 / n2;
            }

            return decimal.Round(result, 2);
        }

        private string SetMesageLastPdv(DateTime? date)
        {
            string message = string.Empty;

            if (date != null)
            {
                message = string.Format("PDVs enviados por última vez el {0} a las {1}", date.Value.ToShortDateString(), date.Value.ToString("hh:mm tt"));
            }
            else
            {
                message = string.Format("No tiene enviados PDVs");
            }

            return message;
        }
    }
}
