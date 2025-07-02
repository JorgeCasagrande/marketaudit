using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IRouteRepository
    {
        IEnumerable<RouteModel> GetRouteByUserIdAndProjectId(long userId, long projectId);
        IEnumerable<RouteModel> GetRouteByUserId(long userId);
        long GetRouteId(long projectId, long userId, long pdvId);
        long GetRoute(long projectId);
        bool Exist(long projectId, long censistId, string name, TransactionalContext transaction);
        Route Get(long projectId, long censistId, string name, TransactionalContext transaction);
        long Insert(Route route, TransactionalContext transaction);
        void InsertRoutePdv(RoutePdv routePdv, TransactionalContext transaction);
        IEnumerable<Filter> GetRouteFilter(long projectId, TransactionalContext transaction);
        void DeleteRoutesProject(long projectId, ITransactionalContext transaction);
    }
}
