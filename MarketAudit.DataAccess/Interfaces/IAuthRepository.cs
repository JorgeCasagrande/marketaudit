using MarketAudit.Entities.Models;

namespace Marketaudit.DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        AuthUser AuthUser(string userName, string password);
    }
}
