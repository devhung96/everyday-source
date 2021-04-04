using Newtonsoft.Json;
using Project.App.Helpers;
using Project.Modules.Groups.Enities;
using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Enities
{
    [Table("hd_bank_tags")]
    public class Tag
    {
        [Key, Column("tag_id")]
        public string TagId { get; set; } = Guid.NewGuid().ToString();

        [Column("tag_name")]
        public string TagName { get; set; }

        [Column("tag_comment")]
        public string TagDescription { get; set; }

        [Column("tag_code")]
        public string TagCode { get; set; }
        [Column("tag_status")]
        public IsAccess TagStatus { get; set; } = IsAccess.Faid;

        [Column("ticket_type_id")]
        public string TicketTypeId { get; set; }


        [Column("tag_repeat_type")]
        public ScheduleRepeatType? TagRepeat { get; set; }

        [Column("tag_repeat_value")]
        public string TagRepeatString { get; set; }

        [NotMapped]
        public List<string> ListRepeatValue
        {
            get
            {
                if (String.IsNullOrEmpty(TagRepeatString))
                {
                    return new List<string>();
                }
                return JsonConvert.DeserializeObject<List<string>>(TagRepeatString);
            }
          

        }



        [Column("tag_date_start")]
        public DateTime? TagDateStart { get; set; }

        [Column("tag_date_end")]
        public DateTime? TagDateEnd { get; set; }


        [Column("time_start")]
        public TimeSpan? TagTimeStart { get; set; }

        [Column("time_end")]
        public TimeSpan? TagTimeEnd { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = GeneralHelper.DateTimeDatabase;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        
        public TicketType TicketType { get; set; }


        [Column("repository_id")]
        public string? RepositoryId { get; set; }
        public enum IsAccess
        {
             Faid =0,
             Success =1,
        }
    }
}
