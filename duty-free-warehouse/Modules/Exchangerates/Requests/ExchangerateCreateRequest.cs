using Project.Modules.Exchangerates.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Exchangerates.Requests
{
    public class ExchangerateCreateRequest
    {
        public string ExchangerateCode { get; set; }
        public double ExchangerateRate { get; set; }
    }
}
