using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Contacts.Requests
{
    public class EditContactRequest
    {
        [Required]
        public string ContactName { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        [Required]
        public string ContactContent { get; set; }
    }
}
