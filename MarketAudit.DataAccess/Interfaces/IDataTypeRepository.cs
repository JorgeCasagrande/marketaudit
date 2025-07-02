using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IDataTypeRepository
    {
        IEnumerable<DataType> GetAll();
        DataType Get(long id);
        DataType Get(string code, ITransactionalContext transaction);
    }
}
