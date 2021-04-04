using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Seals.Requests
{
    public class UpdateStatusRequest
    {
        [Required]
        public int Status { get; set; }
        public DateTime? Date { get; set; }
    }
}
