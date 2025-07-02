using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities.Models.Generic
{

    public class DataTableModel
    {
        public string[] columns { get; set; }
        public object[] data { get; set; }
        public int TotalItems { get; protected set; }
        public int Page { get; protected set; }
        public int TotalPages { get; protected set; }
        public int RowsPerPage { get; protected set; }

        public DataTableModel()
        {

        }

        public DataTableModel(string[] columns, object[] data, int totalItems, int page, int totalPages, int rowsPerPage)
        {
            this.columns = columns;
            this.data = data;
            TotalItems = totalItems;
            Page = page;
            TotalPages = totalPages;
            RowsPerPage = rowsPerPage;
        }
    }
}
