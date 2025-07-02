using MarketAudit.Entities.Models.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class ProjectGrid : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectType { get; set; }
        public string Customer { get; set; }
        public string Responsable { get; set; }
        public string State { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public long CantidadPdv { get; set; }
        public long CantidadCensistas { get; set; }
        public long CantidadPreguntas { get; set; }

    }
}
