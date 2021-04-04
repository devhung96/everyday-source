using Project.Modules.Products.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{  
    [ValidationDeleteProductRequest]
    public class DeleteProductRequest
    {
        public List<string> ProductCode { get; set; }
    }
}

