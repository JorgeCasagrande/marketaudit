using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Request
{
    public class ImportQuestionModel
    {
        public int Orden { get; set; }
        public string Pregunta { get; set; }
        public string Descripcion { get; set; }
        public string Miniatura { get; set; }
        public bool Requerida { get; set; }
        public string Disparadora { get; set; }
        public string TipoPregunta { get; set; }
        public string TipoDato { get; set; }
        public string Respuestas { get; set; }
    }
}
