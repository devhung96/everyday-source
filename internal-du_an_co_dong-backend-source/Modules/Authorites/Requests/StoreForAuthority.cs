using Project.Modules.Authorites.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorites.Requests
{
    [ValidateStoreForAuthority]
    public class StoreForAuthority
    {
        public string EventID { get; set; }
        [Required(ErrorMessage = "UserReceiveIDIsRequired")]
        public string UserReceiveID { get; set; }
        [Required(ErrorMessage = "UserIDIsRequired")]
        public string UserID { get; set; }
    }
}
