using MarketAudit.Entities.Models.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models
{
    public class Project : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long ProjectTypeId { get; set; }
        public long CustomerId { get; set; }
        public long ResponsableId { get; set; }
        public bool? Sas { get; set; }
        public long StateId { get; set; }
        public DateTime Creation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }

        public IList<KeyValueDto> ProjectTypeList { get; set; }
        public IList<KeyValueDto> ResponsableList { get; set; }
        public IList<KeyValueDto> CustomerList { get; set; }
        public IList<KeyValueDto> StatesList { get; set; }
        public IList<KeyValueDto> SizeList { get; set; }

    }
}
