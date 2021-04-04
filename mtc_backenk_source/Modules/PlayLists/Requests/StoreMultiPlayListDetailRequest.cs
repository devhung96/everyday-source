using Project.Modules.PlayLists.Entities;
using Project.Modules.TemplateDetails.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Requests
{
    public class StoreMultiPlayListDetailRequest
    {
        [Required]
        public string PlayListName { get; set; }
        public string PlayListComment { get; set; }
        public PlayListLoopEnum PlayListLoop { get; set; }
        public List<StorePlayListDetailMultipleRequest> storePlayListDetails { get; set; }
        public string PlayListAssignUserId { get; set; }
    }
    public class StorePlayListDetailMultipleRequest
    {
        [CheckTemplateIdValidation]
        public string TemplateId { get; set; }
        public string MediaId { get; set; }
        [Required]
        public TimeSpan? TimeBegin { get; set; }
        [Required]
        public TimeSpan? TimeEnd { get; set; }
        public int OrderMedia { get; set; }
        [Required]
        public TimeSpan TemplateDuration { get; set; }
    }
}
