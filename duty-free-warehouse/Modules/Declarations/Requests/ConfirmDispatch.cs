using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class ConfirmDispatch
    {
        [Required]
        public string DeclaNumber { get; set; }
        [Required]
        public string DispatchNumber { get; set; }
    }
}
