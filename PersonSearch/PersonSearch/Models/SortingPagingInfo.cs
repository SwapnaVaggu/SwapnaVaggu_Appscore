using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonSearch.Models
{
    public class SortingPagingInfo
    {
        public string SortField { get; set; }
        public string SortDirection { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int CurrentPageIndex { get; set; }

        public string name { get; set; }
        public string gender { get; set; }
        public string direction { get; set; }
    }
}