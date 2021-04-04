using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Requests
{
    public class UpdateDestroyDetailRequest
    {
        [Required]
        public int DestroyId { get; set; }
        public List<ChildDestroyDetail> ChildDestroyDetails { get; set; }
        public string DestroyDate { get; set; }
    }
}
