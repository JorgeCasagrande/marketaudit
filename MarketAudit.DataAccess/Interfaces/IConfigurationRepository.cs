using MarketAudit.Entities.Models;
using System;
using System.Collections.Generic;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IConfigurationRepository
    {
        List<Recursos> GetConfiguration(string languaje);
    }
}
