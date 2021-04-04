using Project.App.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    [ValidateRequestTable]
    public class RequestTable
    {
        public int Limit { get; set; } // Số phần tử trong 1 trang
        public int Page { get; set; } // Số trang
        public string SortField { get; set; } // Trường sort
        public string SortOrder { get; set; } // Kiểu sort
        public string Search { get; set; } // Nội dung tìm kiếm 
    }
}
