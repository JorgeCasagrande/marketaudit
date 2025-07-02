using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IPdvRepository
    {
        IEnumerable<PdvModel> GetPdvByRouteId(long routeId);
        int GetCountPdvByProjectIdAndUserId(long projectId, long userId);
        IEnumerable<PdvModel> GetPdvByUserId(long userId);
        IEnumerable<PdvGridProject> GetPdvByProyectId(long projectId);
        long Insert(Entities.Models.PdvEntity pdv, TransactionalContext transaction);
        IEnumerable<Filter> GetPdvFilter(long projectId, string userId, TransactionalContext transaction);
        IEnumerable<ProjectPdvTemplateModel> GetPdvTemplateByProjectId(long projectId);
        long InsertSingle(PdvEntity model, TransactionalContext transaction);
    }
}
