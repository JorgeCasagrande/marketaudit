using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using System.Collections.Generic;

namespace Marketaudit.Service.Interfaces
{
    public interface IUserService
    {
        DataTableModel GetUsers(string roles = null, string states = null);
        ResponseData GetResponsables();

        ResponseData GetRoles();
        ResponseData GetStates();
        ResponseData Enable(long[] ids);
        ResponseData Delete(long[] ids);
        ResponseData Save(User model);
        ResponseData GetNewUser();
        ResponseData GetUser(long id);
    }
}
