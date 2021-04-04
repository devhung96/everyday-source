using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class DeleteTagModeUserRequest
    {
        [Required(ErrorMessage = "UserIdRequired")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "TagIdRequired")]
        public string TagId { get; set; }
        [Required(ErrorMessage = "ModeIdRequired")]
        public string ModeId { get; set; }
    }
}
