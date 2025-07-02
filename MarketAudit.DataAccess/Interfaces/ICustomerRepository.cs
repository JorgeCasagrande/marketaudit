using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers(string states);
        IEnumerable<Customer> GetCustomers(bool enable = true);
        Customer GetCustomer(long id, ITransactionalContext transaction);
        void Enable(long id, int enable, ITransactionalContext transaction);
        void Delete(long id, ITransactionalContext transaction);
        void Update(Customer model, ITransactionalContext transaction);
        void Insert(Customer model, ITransactionalContext transaction);
    }
}
