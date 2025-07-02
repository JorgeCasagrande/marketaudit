using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IResponseRepository
    {
        IEnumerable<Responses> GetResponsesByQuestionId(long projectId, long questionId);
        IEnumerable<Responses> GetResponsesByUserId(long userId);

        ResponseQuestion Get(string response, ITransactionalContext transaction);
        bool Exist(string response, ITransactionalContext transaction);
        long Insert(ResponseQuestion response, ITransactionalContext transaction);
    }
}
