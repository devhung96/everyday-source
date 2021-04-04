using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class UpdateFileRequest
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        public string MediaTitle { get; set; }
    }
}
