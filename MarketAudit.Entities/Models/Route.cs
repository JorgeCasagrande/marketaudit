using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class Route : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long ProjectId { get; set; }
        public long CensistId { get; set; }
        public string Image { get; set; }

        public Route()
        {
        
        }

        public Route(string Route, long ProjectId, long CensistId, string language)
        {
            this.Name = string.Format(translations[language]["Route_Name"], Route);
            this.Description = string.Format(translations[language]["Route_Description"], Route);
            this.ProjectId = ProjectId;
            this.CensistId = CensistId;
            this.Image = "https://weask-images.s3.amazonaws.com/map.png";
        }

        private static readonly Dictionary<string, Dictionary<string, string>> translations =
            new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "es", new Dictionary<string, string>
                    {
                        { "Route_Name", "Ruta {0}" },
                        { "Route_Description", "Circuito {0} para realizar el recorrido de encuentas" }
                    }
                },
                {
                    "pt", new Dictionary<string, string>
                    {
                        { "Route_Name", "Rota {0}" },
                        { "Route_Description", "Circuito {0} para realizar o tour da pesquisa" }
                    }
                },
                {
                    "en", new Dictionary<string, string>
                    {
                        { "Route_Name", "Route {0}" },
                        { "Route_Description", "Circuit {0} to carry out the survey tour" }
                    }
                }
                // Agrega más idiomas y traducciones según sea necesario
            };
    }

}
