using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IReportDetailRepository
    {
        List<ReportDetail> GetReportDetailsByProject(long projectId, ITransactionalContext context);
        List<PhotosReport> GetReportPhotosByProject(long projectId, string users, string pdvs, string routes, string questions, ITransactionalContext context);
        List<ReportModel> GetReportDetailsByProjectId(long projectId);
        void DeleteReport(long projectId, ITransactionalContext transaction);
    }
}
