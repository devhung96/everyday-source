using Project.Modules.Authorities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorites.Requests
{
    public class ChangeStatus
    {
        [Required(ErrorMessage = "AuthorityStatusIsRequired")]
        [EnumDataType(typeof(AuthorityStatus))]
        public AuthorityStatus AuthorityStatus { get; set; }
    }
}
