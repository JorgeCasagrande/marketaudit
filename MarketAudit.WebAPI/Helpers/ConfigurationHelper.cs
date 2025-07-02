using Marketaudit.Entities;
using Marketaudit.WebAPI.Helpers;
using Microsoft.Extensions.Configuration;
using System;

namespace MarketAudit.WebAPI.Helpers
{
    public class ConfigurationHelper
    {
        public static ConfigInfo GetAppConfigurationInfo()
        {
            ConfigInfo configInfo = new ConfigInfo
            {
                ConfigFilename = "App.json"
            };

            return configInfo;
        }

        public static AppConfiguration GetAppConfiguration(string configurationFile = "App.json")
        {
            var configInfo = GetAppConfigurationInfo();
            configurationFile = configInfo.ConfigFilename;
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile(configurationFile, optional: false)
                .Build();
            var result = configuration.Get<AppConfiguration>();

            return result;
        }
    }
}
