using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Requests
{
    [ValidateRequestTable]
    public class RequestTable
    {
        public int Results { get; set; }
        public int Page { get; set; }
        public string SortField { get; set; } 
        public string SortOrder { get; set; } 
        public string Search { get; set; }
        public string SearchGroup { get; set; } // search theo group 
        public int Type { get; set; }
    }
}
