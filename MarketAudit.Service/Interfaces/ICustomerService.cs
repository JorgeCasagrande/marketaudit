using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using System.Collections.Generic;

namespace Marketaudit.Service.Interfaces
{
    public interface ICustomerService
    {
        DataTableModel GetCustomers(string states = null);
        ResponseData GetStates();
        ResponseData Enable(long[] ids);
        ResponseData Delete(long[] ids);
        ResponseData Save(Customer model);
        ResponseData GetNewCustomer();
        ResponseData GetCustomer(long id);
    }
}
