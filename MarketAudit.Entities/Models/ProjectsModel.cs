using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class ProjectsModel : Entity
    {
        public string Name { get; set; }
        public string Client { get; set; }
        public string ProjectType { get; set; }
        public int TotalPdv { get; set; }
        public int SendedPdv { get; set; }
        public string ProjectStatus { get; set; }
    }
}
