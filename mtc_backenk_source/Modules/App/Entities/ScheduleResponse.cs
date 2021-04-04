using Project.Modules.PlayLists.Entities;
using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.App.Entities
{
    public class ScheduleResponse
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public TimeSpan TimeBegin { get; set; }
        public TimeSpan TimeEnd { get; set; }


        public ScheduleSequential ScheduleSequential { get; set; } = ScheduleSequential.Off;

        public string PlayListName { get; set; }
        public string PlayListId { get; set; }

        public PlayListLoopEnum PlaylistLoop { get; set; }
        public List<string> ScheduleIds { get; set; } = new List<string>();
       
        public List<TemplateResponse> Templates { get; set; }

    }


    public class TemplateResponse
    {
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }

        public float? TempRatioX { get; set; }
        public float? TempRatioY { get; set; }

        public TimeSpan TimeBegin { get; set; }
        public TimeSpan TimeEnd { get; set; }

        public TimeSpan? TemplateDuration { get; set; }
        public List<MediaResponse> Medias { get; set; }
    }

    public class MediaResponse
    {
        public string MediaId { get; set; }

        public string MediaUrl { get; set; }
        public string MediaMd5 { get; set; }
        public string MediaTypeCode { get; set; }

        
       
        public float TempRatioX { get; set; }
        public float TempRatioY { get; set; }
        public double TempPointWidth { get; set; }
        public double TempPointHeight { get; set; }
        public double TempPointTop { get; set; }
        public double TempPointLeft { get; set; }
        public double TempDetailZindex { get; set; }

        public PlayListLoopEnum PlaylistLoop { get; set; }

        public string ScheduleId { get; set; }
    }
}
