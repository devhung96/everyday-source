using Project.Modules.Products.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{
    [ValidationDeleteMenus]
    public class DeleteMenusRequest
    {
        public List<int> MenuIds { get;set; }
    }
}
