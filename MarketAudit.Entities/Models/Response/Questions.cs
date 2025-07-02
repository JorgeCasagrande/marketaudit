using MarketAudit.Entities;
using System.Collections.Generic;

namespace Marketaudit.Entities.Models.Response
{
    public class Questions : Entity
    {
        public string Question { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string QuestionType { get; set; }
        public bool Required { get; set; }
        public string QuestionImg { get; set; }
        public int Order { get; set; }
        public List<Responses> Responses { get; set; }
        public Answer Answer { get; set; }
        public string Units { get; set; }
    }
}
