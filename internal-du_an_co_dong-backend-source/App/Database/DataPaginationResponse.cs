using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Database
{
    public class DataPaginationResponse
    {
        public double total { get; set; }
        public double limit { get; set; }
        public double lastePage { get; set; }
        public double page { get; set; }
        public object[] data { get; set; }
    }
}
