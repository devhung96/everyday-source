using Project.Modules.RegisterDetects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.DeviceTypes.Entities.DeviceType;
using static Project.Modules.Tags.Entities.Tag;

namespace Project.Modules.Tags.Requests
{
    public class AddTagRequest
    {
        public string TicketTypeId { get; set; }

        public string TagName { get; set; }
        public string TagCode { get; set; }
        public string TagDescription { get; set; }

        public string TagTimeStart { get; set; }
        public string TagTimeEnd { get; set; } 

        public DateTime? TagDateStart { get; set; }
        public DateTime? TagDateEnd { get; set; }

        
        public RepeatType? TagRepeat { get; set; }
        public string TagRepeatValue { get; set; }
        public List<string> ListRepeatValue { get; set; }

        public STATUS? TagStatus { get; set; }
    }
}
