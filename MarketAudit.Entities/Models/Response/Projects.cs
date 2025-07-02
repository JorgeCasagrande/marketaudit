using MarketAudit.Entities;
using MarketAudit.Entities.Models.Response;
using System.Collections.Generic;

namespace Marketaudit.Entities.Models.Response
{
    public class Projects : Entity
    {
        public string Name { get; set; }
        public string Client { get; set; }
        public string ProjectType { get; set; }
        public int TotalPdv { get; set; }
        public int SendedPdv { get; set; }
        public string ProjectStatus { get; set; }
        public bool Change { get; set; }
        public List<Routes> Routes { get; set; }
        public List<Questions> Questions { get; set; }
    }
}
