using Marketaudit.Service.Interfaces;
using Marketaudit.Service.Services;
using MarketAudit.Service.Interfaces;
using MarketAudit.Service.Services;

namespace MarketAudit.Service
{
    public class ServiceFactory
    {
        public static IAuthService GetClasificacionLogic()
        {
            return new AuthService();
        }
        public static IProjectService GetUsuarioLogic()
        {
            return new ProjectService();
        }
        public static IReportService GetReportService()
        {
            return new ReportService();
        }
        public static ICustomerService GetCustomerService()
        {
            return new CustomerService();
        }

        public static IUserService GetUserService()
        {
            return new UserService();
        }

        public static ILogAppService GetLogAppService()
        {
            return new LogAppService();
        }

        public static IConfigurationService GetConfigurationService()
        {
            return new ConfigurationService();
        }
    }
}
