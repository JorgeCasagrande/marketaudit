using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class Question : Entity
    {
        public string QuestionName { get; set; }
        public string Description { get; set; }
        public long QuestionTypeId { get; set; }
        public long DataTypeId { get; set; }
        public bool Required { get; set; }
        public string Image { get; set; }

    }
}
