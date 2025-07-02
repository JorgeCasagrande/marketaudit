using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class LogAppMk
    {
        public long? UserId { get; set; }
        public long? ProjectId { get; set; }
        public long? PdvId { get; set; }
        public string Message { get; set; }
    }
}
