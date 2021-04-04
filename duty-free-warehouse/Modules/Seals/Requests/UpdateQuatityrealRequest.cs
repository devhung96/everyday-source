using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Seals.Requests
{
    public class UpdateQuatityrealRequest
    {
        [Required]
        public int quantityReal { get; set; }
    }
}
