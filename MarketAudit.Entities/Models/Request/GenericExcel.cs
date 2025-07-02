using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Request
{
    public class GenericExcel
    {
        public bool ContainError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
