using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class RoutePdv : Entity
    {
        public long RouteId { get; set; }
        public long PdvId { get; set; }
    }
}
