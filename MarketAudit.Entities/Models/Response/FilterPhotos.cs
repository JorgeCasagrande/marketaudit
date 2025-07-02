using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Response
{
    public class FilterPhotos
    {
        public List<Filter> Users { get; set; }
        public List<Filter> Pdvs { get; set; }
        public List<Filter> Routes { get; set; }
        public List<Filter> Quesions { get; set; }
    }
}
