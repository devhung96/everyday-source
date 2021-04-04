using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Entities
{
    public class ReportInventory
    {
        public string productName { get; set; }
        public string productCode { get; set; }
        public string productUnit { get; set; }
        public int amountInventory { get; set; }
        public int amountExport { get; set; }
        public int amountDestroy { get; set; }
        public int sum { get; set; }
        public DateTime createAt { get; set; }
    }
}
