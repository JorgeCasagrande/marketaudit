using Marketaudit.Entities.Models.Response;
using System.Collections.Generic;

namespace MarketAudit.Entities.Models.Response
{
    public class Login
    {
        public long UserId { get; set; }
        public bool Change { get; set; }
        public string UserName { get; set; }
        public List<Projects> Projects { get; set; }
    }
}
