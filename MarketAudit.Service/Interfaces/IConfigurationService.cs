using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Service.Interfaces
{
    public interface IConfigurationService
    {
        List<Recursos> GetConfiguration(string language);
    }
}
