using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Response
{
    public class ProjectReport
    {
        public long Id { get; }
        public string Value { get; }
        public string Icon { get; set; }

        public ProjectReport(long id, string value)
        {
            Id = id;
            Value = value;
            Icon = string.Empty;
        }
    }
}
