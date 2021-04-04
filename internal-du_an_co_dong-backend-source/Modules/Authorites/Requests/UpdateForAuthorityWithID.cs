using Project.Modules.Authorites.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorites.Requests
{
    [ValidateUpdateForAuthority]
    public class UpdateForAuthorityWithID
    {
        [Required(ErrorMessage = "AuthorityIDIsRequired")]
        public int? AuthorityID { get; set; }
        [Required(ErrorMessage = "UserReceivIsRequired")]
        public string UserReceive { get; set; }
    }
}
