using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Response
{
    public class ReportPdv
    {
        public long ProjectId { get; set; }
        public string  Name { get; set; }
        public int PdvPendientes { get; set; }
        public decimal AvanceCampo { get; set; }
        public decimal PromedioDiario { get; set; }
        public decimal Tendencia { get; set; }
        public string LastSendMessage { get; set; }
        public decimal ConsumedTime { get; set; }
    }
}
