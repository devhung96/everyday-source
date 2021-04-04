using Project.App.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Requests
{
    [ValidateReqeustTable]
    public class RequestTable
    {
        public int results { get; set; }
        public int page { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
        public string search { get; set; }
    }
}
