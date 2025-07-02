using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using System.Collections.Generic;

namespace Marketaudit.Service.Interfaces
{
    public interface IReportService
    {
        ResponseData CreateReport(InfoProject model);
        IEnumerable<ReportPdv> GetReportPdv(long userId);
    }
}
