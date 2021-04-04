using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Requests
{
    public class TranscriptByClassRequest
    {
        [Required(ErrorMessage = "ClassIdIsRequired")]
        [MaxLength(36, ErrorMessage = "MaxLength36Character")]
        public string ClassId { get; set; }


        [Required(ErrorMessage = "SubjectIdIsRequired")]
        [MaxLength(255, ErrorMessage = "MaxLength255Character")]
        public string SubjectId { get; set; }

        [Required(ErrorMessage = "SemestersIdIsRequired")]
        [MaxLength(255, ErrorMessage = "MaxLength255Character")]
        public string SemestersId { get; set; }


        public string Search { get; set; }
    }
}
