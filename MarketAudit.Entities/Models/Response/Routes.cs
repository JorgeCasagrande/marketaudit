using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Response
{
    public class Routes : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Map { get; set; }
        public bool Change { get; set; }
        public List<Pdv> Pdvs { get; set; }
    }
}
