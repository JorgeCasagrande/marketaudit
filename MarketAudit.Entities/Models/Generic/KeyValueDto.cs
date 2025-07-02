using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Generic
{
    public class KeyValueDto
    {
        public long Key { get; }
        public string Value { get; }
        public string Description { get; set; }

        public KeyValueDto(long key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
