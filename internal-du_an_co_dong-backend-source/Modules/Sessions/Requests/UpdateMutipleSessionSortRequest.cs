using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sessions.Requests
{
    public class UpdateMutipleSessionSortRequest
    {
        public List<ItemUpdateMutipleSessionSortRequest> Sessions { get; set; }
    }


    public class ItemUpdateMutipleSessionSortRequest
    {
        [Required(ErrorMessage = "SessionIdIsRequired")]
        public string SessionId { get; set; }
        [Required(ErrorMessage = "SessionSortIsRequired")]
        public int SessionSort { get; set; }
    }
}
