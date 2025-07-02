using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IReportRepository
    {
        void Create(long userId, long projectId, long pdvId, long routeId, ITransactionalContext transaction);
        long GetReportMasterId(long userId, long projectId, long pdvId, long routeId, ITransactionalContext transaction);
        void CreateDetail(long reportMasterId, long projectId, ITransactionalContext transaction);
        void UpdateDetail(long reportMasterId, long questionId, int order, string Value, ITransactionalContext transaction);
        bool ExistReport(long userId, long projectId, long pdvId, long routeId, ITransactionalContext transaction);
        ReportPdv GetReportPdv(long projectId, long userId);
        int GetOrderQuestion(long questionId, ITransactionalContext transaction);
    }
}
