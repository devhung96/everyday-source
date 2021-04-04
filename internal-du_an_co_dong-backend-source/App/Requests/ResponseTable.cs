using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Requests
{
    //[ValidateRequestTable]
    public class ResponseTable
    {
        public object DateResult { get; set; }
        public Info Info { get; set; }
        public object Total { get; set; }
    }

    public class Info
    {
        public int Results { get; set; } // số lượng item trong một trang
        public int Page { get; set; }
        public int TotalRecord { get; set; }
    }
}
