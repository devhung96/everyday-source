using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;

namespace Project.Modules.Seals.Response
{
    public class TableResponse
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public long TotalRecord { get; set; }
        public long TotalProduct { get; set; }
        public IEnumerable<dynamic> Data { get; set; }
    }
}
