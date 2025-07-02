using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class AuthUser
    {
        public bool Auth { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; }
    }
}
