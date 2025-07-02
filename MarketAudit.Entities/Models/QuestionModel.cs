using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class QuestionModel : Entity
    {
        public long QuestionId { get; set; }
        public long ProjectId { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string QuestionType { get; set; }
        public bool Required { get; set; }
        public string QuestionImg { get; set; }
        public int Order { get; set; }
    }
}
