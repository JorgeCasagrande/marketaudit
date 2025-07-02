using MarketAudit.Common.Log;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MarketAudit.Service.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public IConfigurationRepository repository;
        public ConfigurationService()
        {
            repository = new ConfigurationRepository();
        }

        public List<Recursos> GetConfiguration(string language)
        {
            return repository.GetConfiguration(language);
        }
    }
}
