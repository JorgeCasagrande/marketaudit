using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Request
{
    public class ImportPdvModel
    {
        public string Censist { get; set; }
        public string Name { get; set; }
        public long Number { get; set; }
        public string Cuit { get; set; }
        public string Address { get; set; }
        public string PdvType { get; set; }
        public string Route { get; set; }
        public string Notes { get; set; }
        public string Language { get; set; }
    }
}
