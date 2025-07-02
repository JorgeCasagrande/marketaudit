using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class UserModel
    {
        public long UserId { get; set; }
        public bool IsEnabled { get; set; }
    }
}
