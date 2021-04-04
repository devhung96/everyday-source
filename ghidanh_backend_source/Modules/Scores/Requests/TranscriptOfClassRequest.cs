using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Requests
{
    public class TranscriptOfClassRequest
    {
        [Required]
        public string ClassId { get; set; }

        public string SemestersId { get; set; }
    }
}
