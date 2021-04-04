using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class RegisterMeetingWithTagMode
    {
        public string UserId { get; set; }
        public List<TagMode> TagModes = new List<TagMode>();
    }
    public class TagMode
    {
        public string TagId { get; set; }
        public string ModeId { get; set; }
    }
}
