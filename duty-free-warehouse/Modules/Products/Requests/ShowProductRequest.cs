using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{
    public class ShowProductRequest
    {
        public string ContentSearch { get; set; } = "";
    }
}
