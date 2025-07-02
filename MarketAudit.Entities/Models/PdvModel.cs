using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class PdvModel : Entity
    {
        public long RouteId { get; set; }
        public long PdvNumber { get; set; }
        public string PdvName { get; set; }
        public string PdvDescription { get; set; }
        public string Address { get; set; }
        public bool Completed { get; set; }
    }
}
