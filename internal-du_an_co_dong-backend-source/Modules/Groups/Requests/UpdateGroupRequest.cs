using Project.App.Helpers;
using Project.Modules.Functions.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Requests
{
    public class UpdateGroupRequest
    {
        //[ValidateGroupName]
        //[Required(ErrorMessage =" không được rỗng.")]
        public string GroupName { get; set; }
        public List<ModuleItemRequest> pemissionGroups { get; set; }
    }
}
