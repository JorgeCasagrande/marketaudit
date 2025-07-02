using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface ITransactionalContext
    {
        void Commit();
        void Rollback();
        SqlCommand GetCommand();
    }
}
