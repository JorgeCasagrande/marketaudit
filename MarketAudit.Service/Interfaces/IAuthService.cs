using MarketAudit.Entities.Models;

namespace Marketaudit.Service.Interfaces
{
    public interface IAuthService
    {
        AuthUser AuthUser(string userName, string password);
    }
}
