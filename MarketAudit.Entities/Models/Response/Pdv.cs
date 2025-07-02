using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Response
{
    public class Pdv : Entity
    {
        public long Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public bool Change { get; set; }
        public bool Completed { get; set; }
    }
}
