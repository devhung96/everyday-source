using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Entities
{

    public class ScheduleResponsePrepare
    {
        public DateTime Key { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
    
    
    public class ScheduleResponse
    {
        public DateTime date { get; set; }
        public DateTime time_begin { get; set; }
        public DateTime time_end { get; set; }
        public List<MediaResponse> medias { get; set; }

    }

    public class MediaResponse
    {
        public string media_name { get; set; }
        public string media_md5 { get; set; }
        public string media_url { get; set; }
        public string media_id { get; set; }
        public int playlist_loop { get; set; }
        public DateTime time_begin { get; set; }
        public DateTime time_end { get; set; }
        public double temp_detail_top { get; set; }
        public double temp_detail_bottom { get; set; }
        public double temp_detail_left { get; set; }
        public double temp_detail_right { get; set; }
        public int temp_detail_zindex { get; set; }
        public int order_media { get; set; }



    }
}
