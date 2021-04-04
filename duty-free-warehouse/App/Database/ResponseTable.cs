using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Database
{
    public class ResponseTable
    {
        public object results { get; set; }
        public Info info { get; set; }
        public object total { get; set; } // Thêm để hỗ trợ frotend
    }
    public class Info
    {
        public int results { get; set; }
        public int page { get; set; }
        public int totalRecord { get; set; }
    }
}
