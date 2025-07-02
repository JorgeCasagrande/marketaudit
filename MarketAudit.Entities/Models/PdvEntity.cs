using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class PdvEntity : Entity
    {
        public string Name { get; set; }
        public long Number { get; set; }
        public string Description { get; set; }
        public string Cuit { get; set; }
        public string Address { get; set; }
        public long PdvTypeId { get; set; }
        public string Notes { get; set; }
        public string Visible { get; set; }
    }
}
