using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class ProjectQuestion : Entity
    {
        public long ProjectId { get; set; }
        public long QuestionId { get; set; }
        public int Order { get; set; }
    }
}
