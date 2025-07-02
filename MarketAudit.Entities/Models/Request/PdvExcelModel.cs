using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Request
{
    public class PdvExcelModel : GenericExcel
    {
        public List<ImportPdvModel> List { get; set; }
    }
}
