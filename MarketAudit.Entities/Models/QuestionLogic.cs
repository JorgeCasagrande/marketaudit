using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class QuestionLogic : Entity
    {
        public long QuestionId { get; set; }
        public long ResponseId { get; set; }
        public int Value { get; set; }
    }
}
