using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Requests
{
    public class StoreTemplateDefaultRequest
    {
        [Required]
        public string MediaId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
