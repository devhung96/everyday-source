using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorites.Requests
{
    public class GetUserForAuthority
    {
        [Required(ErrorMessage = "ShareHolderCodeIsRequired")]
        public string ShareHolderCode { get; set; }
        public string EventID { get; set; }
    }
}
