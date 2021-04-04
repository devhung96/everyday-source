
using Microsoft.Extensions.Configuration;
using Project.App.Helpers;
using Project.Modules.Organizes.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Entities
{
    [Table("shareholder_event")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public string EventId { get; set; } 

        [Column("event_name")]
        public string EventName { get; set; } 


        [Column("event_time_begin")]
        public DateTime EventTimeBegin { get; set; } 


        [Column("event_time_end")]
        public DateTime EventTimeEnd { get; set; }


        [Column("event_logo_url")]
        public string EventLogoUrl { get; set; }


        [Column("event_type")]
        public int? EventType { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }


        [Column("event_status")]
        [EnumDataType(typeof(EVENT_STATUS))]
        public EVENT_STATUS EventStatus { get; set; } = EVENT_STATUS.ACTIVED;

        [Column("event_count_down")]
        [EnumDataType(typeof(EVENT_COUNT_DOWN))]
        public EVENT_COUNT_DOWN EventCountDown { get; set; } = EVENT_COUNT_DOWN.ALBE;


        [Column("organize_id")]
        public string OrganizeId { get; set; }


        [Column("event_session_default")]
        public string EventSessionDefault { get; set; }


        public Organize Organize { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }


        [Column("event_flag")]
        [EnumDataType(typeof(EVENT_FLAG))]
        public EVENT_FLAG EventFlag { get; set; } = EVENT_FLAG.CREATED;

        [Column("event_setting")]
        public string EventSetting { get; set; }


    }

    public enum EVENT_STATUS
    {
        ACTIVED =1 ,
        DELETED  = 0
    }

    public enum EVENT_FLAG
    {
        CREATED = 0,
        BEGIN = 1,
        END = 2
    }


    public enum EVENT_COUNT_DOWN
    {
        UNALBE = 0,
        ALBE = 1
    }
}
