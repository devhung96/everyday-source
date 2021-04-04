using Project.Modules.Products.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{
    [ValidationAddMenu]
    public class AddMenuRequest
    {
        [Required]
        public string ProductCode { get; set; }
        [Required]
        public int MenuId { get; set; } = 1;
    }
}
