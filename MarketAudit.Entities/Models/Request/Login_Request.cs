using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Request
{
    public class Login_Request
    {
        public string User { get; set; }
        public string Password { get; set; }
    }
}
