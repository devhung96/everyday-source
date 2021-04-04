using Project.App.Request;
using Project.Modules.Medias.Validations;
using Project.Modules.PlayLists.Entities;
using Project.Modules.PlayLists.Validations;
using Project.Modules.TemplateDetails.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Requests
{
    public class StorePlayListDetailRequest 
    {
        [Required, CheckPlayListIDValidation]
        public string PlayListId { get; set; }
        [Required]
        public string TemplateId { get; set; }
        [Required]
        public TimeSpan? TimeBegin { get; set; }
        [Required]
        public TimeSpan? TimeEnd { get; set; }
        public int OrderMedia { get; set; }
        [Required]
        public TimeSpan TemplateDuration { get; set; }
    }
}
