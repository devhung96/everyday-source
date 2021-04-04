using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorites.Requests
{
    public class UpdateAuthority
    {
        [Required(ErrorMessage = "EventIDIsRequired")]
        public string EventID { get; set; }
        [Required(ErrorMessage = "UserReceiveIDIsRequired")]
        public string UserReceiveID { get; set; }
        [Required(ErrorMessage = "UserIDIsRequired")]
        public string UserID { get; set; }
    }
}
