using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Aws.Requests
{
    public class DeleteFileRequest
    {
        [Required]
        public string Key { get; set; }
    }
}
