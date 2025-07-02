using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IProjectQuestionRepository
    {
        DataTable GetColumnsReport(long projectId, ITransactionalContext context);
    }
}
