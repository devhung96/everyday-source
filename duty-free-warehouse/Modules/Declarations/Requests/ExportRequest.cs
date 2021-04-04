using MongoDB.Bson.IO;
using Project.Modules.Destroys.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class ExportRequest
    {
        [ValidateStringDateTime]
        public string DateFrom { get; set; }
        [ValidateStringDateTime]
        public string DateTo { get; set; }

        public DateTime? DeDateFrom { get; set; }
        public DateTime? DeDateTo { get; set; }
    }
}
