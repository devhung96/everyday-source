using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class UserByTagPagination: PaginationRequest
    {
        [Required]
        public string TagId { get; set; }
    }
}
