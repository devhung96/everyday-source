using Project.Modules.RegisterDetects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Requests
{
    public class UnRegisterUserDetectRequest
    {
        public string RgDectectUserId { get; set; }
        public string RgDectectKey { get; set; }

        public string ModeId { get; set; }
        public string TagCode { get; set; }

        public string TicketTypeId { get; set; }


        //detail 
        public string RgDectectDetailDateBegin { get; set; }
        public string RgDectectDetailDateEnd { get; set; }

        public string RgDectectDetailTimeBegin { get; set; }
        public string RgDectectDetailTimeEnd { get; set; }

        public RepeatType RgDectectDetailRepeat { get; set; }
        public List<string> RgDectectDetailRepeatValueData { get; set; }



    }
}
