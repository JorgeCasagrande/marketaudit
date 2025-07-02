using MarketAudit.Entities.Models;
using System;
using System.Collections.Generic;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface ILogAppRepository
    {
        IEnumerable<LogApp> GetByDate(string date);
        void InsertLogAppMk(LogAppMk model, ITransactionalContext transaction);
    }
}
