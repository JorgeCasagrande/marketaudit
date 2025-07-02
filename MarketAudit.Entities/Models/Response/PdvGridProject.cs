using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Response
{
    public class PdvGridProject
    {
        public long Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Cuit { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public string RouteName { get; set; }
        public string RouteDescription { get; set; }
        public string Censist { get; set; }
    }
}
