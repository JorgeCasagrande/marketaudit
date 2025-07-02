using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Generic
{
    public class ResponseData
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public object Data { get; set; }
    }
}
