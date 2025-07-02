using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers(string roles,string states);
        User Get(long id);
        User Get(string userName,ITransactionalContext transaction);
        void Enable(User user, ITransactionalContext transaction);
        void Delete(long id, ITransactionalContext transaction);
        void Update(User model, ITransactionalContext transaction);
        void Insert(User model, ITransactionalContext transaction);
        IEnumerable<User> GetResponsables();
        IEnumerable<Filter> GetUserFilter(long projectId, TransactionalContext transaction);
    }
}
