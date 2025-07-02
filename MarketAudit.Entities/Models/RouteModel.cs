using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class RouteModel : Entity
    {
        public long projectId { get; set; }
        public string RouteName { get; set; }
        public string RouteDescription { get; set; }
        public string Map { get; set; }
    }
}
