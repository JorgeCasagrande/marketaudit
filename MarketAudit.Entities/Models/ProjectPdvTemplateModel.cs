using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class ProjectPdvTemplateModel
    {
        public string Id { get; set; }
        public string Censista { get; set; }
        public string Nombre { get; set; }
        public long Numero { get; set; }
        public string Cuit { get; set; }
        public string Direccion { get; set; }
        public string Tipo { get; set; }
        public string Ruta { get; set; }
        public string Visible { get; set; }
    }
}
