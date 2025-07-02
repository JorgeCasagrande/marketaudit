using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Request
{
    public class QuestionExcelModel : GenericExcel
    {
        public List<ImportQuestionModel> List { get; set; }
    }
}
