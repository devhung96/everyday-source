using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SettlementReports.Requests
{
    public class InputFiterRequest
    {
        public string Search { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int? perPage { get; set; }
        public int? page { get; set; }

    }
}
