using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SubjectGroups.Requests
{
    public class StoreSubjectGroupRequest
    {
        [Required]
        public string SubjectGroupName { get; set; }
    }
}
