using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Requests
{
    public class UserTicketRequest : PaginationRequest
    {
        [Required]
        public string TagCode { get; set; }
    }
}
