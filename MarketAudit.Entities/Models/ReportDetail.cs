using Marketaudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class ReportDetail : Entity
    {
        public long ReportMasterId { get; set; }
        public long QuestionId { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        public long StateId { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public long PdvId { get; set; }
        public long RouteId { get; set; }

        public DateTime Creation { get; set; }
        public long ReportMasterStateId { get; set; }

        public virtual ReportMaster ReportMaster { get; set; }
        public virtual QuestionModel Question { get; set; }

    }
}
