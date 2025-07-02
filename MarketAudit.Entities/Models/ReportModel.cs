using Marketaudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class ReportModel
    {
        public string Value { get; set; }
        public int Order { get; set; }
        public string UserName { get; set; }
        public string PdvNumber { get; set; }
        public string PdvName { get; set; }
        public string Route { get; set; }
        public DateTime Creation { get; set; }
        public long ReportMasterId { get; set; }
    }
}
