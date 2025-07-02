using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Common.GlobalVariables
{
    public class Constantes
    {
        public class QuestionExcelFields
        {
            public const string Orden = "Orden";
            public const string Pregunta = "Pregunta";
            public const string Descripcion = "Descripcion";
            public const string Miniatura = "Miniatura";
            public const string Requerida = "Requerida";
            public const string Disparadora = "Disparadora";
            public const string TipoPregunta = "TipoPregunta";
            public const string TipoDato = "TipoDato";
            public const string Respuestas = "Respuestas";
        }

        public class PdvExcelFields
        {
            public const string Censist = "Censist";
            public const string Name = "Name";
            public const string Number = "Number";
            public const string Cuit = "Cuit";
            public const string Address = "Address";
            public const string PdvType = "PdvType";
            public const string Route = "Route";
            public const string Notes = "Notes";
            public const string Language = "Language";
        }

        public class OTExcelDataType
        {
            public const string Numeric = "Numeric";
            public const string String = "String";
            public const string Bool = "Bool";
        }
    }
}
