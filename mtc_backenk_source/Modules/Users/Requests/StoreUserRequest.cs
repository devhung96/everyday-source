using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class StoreUserRequest
    {
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage = "UserEmailInValid")]
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength= 6)]
        public string UserPass { get; set; }
        public string UserPassConfirm { get; set; }
        public UserLevelEnum UserLevel { get; set; } = UserLevelEnum.SUPERADMIN;
        public DateTime? ExpiredAt { get; set; } = null;
        public string GroupId { get; set; }
        public string RoleId { get; set; }
       
    }
}

