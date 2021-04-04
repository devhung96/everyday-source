using Project.Modules.RegisterDetects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Requests
{
    public class RegisterDettectDetailRequest
    {
        public string RgDectectDetailDateBegin { get; set; }
        public string RgDectectDetailDateEnd { get; set; }


        public string RgDectectDetailTimeBegin { get; set; }
        public string RgDectectDetailTimeEnd { get; set; }


        public RepeatType RgDectectDetailRepeat { get; set; }
        public List<string> RgDectectDetailRepeatValueData { get; set; }

    }
}
