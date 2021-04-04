using Project.App.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{
    public class UpdateProductRequest: BaseRequest<UpdateProductRequest>
    {
        public string ProductName { get; set; }
        public string ProductUnit { get; set; }
        public string ProductType { get; set; }
        public int? ProductParLevel { get; set; }
    }
}
