using Project.Modules.Declarations.Validatations;
using Project.Modules.Destroys.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class WarehouseReceiptRequest
    {
        [ValidationDateTimeAttribute]
        public string DateFrom { get; set; }
        [ValidationDateTimeAttribute]
        public string DateTo { get; set; }
        public string DeclarationNumber { get; set; }
    }
}
