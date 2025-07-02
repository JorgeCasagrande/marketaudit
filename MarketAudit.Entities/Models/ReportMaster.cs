using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class ReportMaster : Entity
    {
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public long PdvId { get; set; }
        public long RouteId { get; set; }

        public DateTime Creation { get; set; }
        public long StateId { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
        public virtual PdvEntity Pdv { get; set; }
        public virtual Route Route { get; set; }
    }
}
