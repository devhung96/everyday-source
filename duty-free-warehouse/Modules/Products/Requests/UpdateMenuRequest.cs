using Project.App.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{
    public class UpdateMenuRequest:BaseRequest<UpdateMenuRequest>
    {
        public int? MenuDetailParlever { get; set; }
        public int? MenuDetailOrder { get; set; }
    }
}
