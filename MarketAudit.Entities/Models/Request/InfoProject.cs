using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Request
{
    public class InfoProject
    {
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public List<PdvInfoProject> Pdvs { get; set; }
    }

    public class PdvInfoProject
    {
        public long PdvId { get; set; }
        public List<QuestionInfoProject> Questions { get; set; }
    }

    public class QuestionInfoProject 
    {
        public long QuestionId { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}
