using Marketaudit.DataAccess.Interfaces;
using Marketaudit.Service.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;

namespace MarketAudit.Service.Services
{
    public class AuthService: IAuthService
    {
        public IAuthRepository repository;
        public AuthService()
        {
            repository = new AuthRepository();
        }

        public AuthUser AuthUser(string userName, string password)
        {
            var result = repository.AuthUser(userName, password);
            return result;
        }
    }
}
