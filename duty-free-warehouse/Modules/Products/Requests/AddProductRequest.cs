using Project.Modules.Products.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{
    public class AddProductRequest
    {
        [Required]
      //  [ValidateProductCode]

        public string ProductCode { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductUnit { get; set; }
        public string ProductType { get; set; }
        [Required]
        public int ProductParLevel { get; set; } = 1;
        public int Status { get; set; } = 1;
        // Status = 1 mới hoàn toàn
        // Status = 2 code đã tồn tại 
        // Status = 3 update

    }
}
