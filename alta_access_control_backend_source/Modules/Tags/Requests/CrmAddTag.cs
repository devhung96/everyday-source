using Project.Modules.RegisterDetects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.DeviceTypes.Entities.DeviceType;

namespace Project.Modules.Tags.Requests
{
    public class CrmAddTag
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public string TagCode { get; set; }
        public string TagDescription { get; set; }
        public string TicketTypeId { get; set; }
        public string TagTimeStart { get; set; }
        public string TagTimeEnd { get; set; }
        public string TagDateStart { get; set; }
        public string TagDateEnd { get; set; }
        public RepeatType? TagRepeat { get; set; }
        public string RepeatValue { get; set; }
        public STATUS? TagStatus { get; set; }

    }

    public class CrmRemoveTag
    {
        public string TagCode { get; set; }
    }
}