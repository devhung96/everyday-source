using Newtonsoft.Json;
using Project.Modules.RegisterDetects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.DeviceTypes.Entities.DeviceType;

namespace Project.Modules.Tags.Entities
{
    [Table("hdbank_ac_tags")]
    public class Tag
    {
        [Key]
        [Column("tag_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TagId { get; set; }

        [Column("ticket_type_id")]
        public string? TicketTypeId { get; set; }


        [Column("repository_id")]
        public string RepositoryId { get; set; }


        [Column("tag_name")]
        public string TagName { get; set; }
        [Column("tag_code")]
        public string TagCode { get; set; }
        [Column("tag_description")]
        public string TagDescription { get; set; }

        [Column("tag_time_start")]
        public TimeSpan? TagTimeStart { get; set; }
        [Column("tag_time_stop")]
        public TimeSpan? TagTimeEnd { get; set; }


        [Column("tag_date_start")]
        public DateTime? TagDateStart { get; set; }
        [Column("tag_date_stop")]
        public DateTime? TagDateEnd { get; set; }


        [Column("tag_repeat")]
        public RepeatType TagRepeat { get; set; } = RepeatType.NonRepeat;
        [Column("tag_value")]
        public string TagRepeatValue { get; set; }

        [NotMapped]

        public List<string> ListRepeatValue
        {
            get
            {
                if (String.IsNullOrEmpty(TagRepeatValue))
                {
                    return new List<string>();
                }
                return JsonConvert.DeserializeObject<List<string>>(TagRepeatValue);
            }
           
        }


        [Column("tag_type")]
        public TAG_TYPE TagType { get; set; } = TAG_TYPE.AccessControll;

        [Column("tag_status")]
        public STATUS TagStatus { get; set; } = STATUS.Active;
        
        [Column("tag_created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;


        public enum TAG_TYPE
        {
            AccessControll = 0,
            CRM = 1
        }
        public Tag() { }
        //public Tag(Tag tag)
        //{
        //    TagId = tag.TagId;
        //    TicketTypeId = tag.TicketTypeId;
        //    TagName = tag.TagName;
        //    TagCode = tag.TagCode;
        //    TagDescription = tag.TagDescription;
        //    TagStatus = tag.TagStatus;
        //    TagType = tag.TagType;
        //    TagTimeStart = tag.TagTimeStart;
        //    TagTimeEnd = tag.TagTimeEnd;
        //    TagDateStart = tag.TagDateStart;
        //    TagDateEnd = tag.TagDateEnd;
        //    TagRepeatValue = tag.TagRepeatValue;
        //    TagRepeat = tag.TagRepeat;
        //    ListRepeatValue = tag.TagRepeatValue != null ? JsonConvert.DeserializeObject<List<string>>(tag.TagRepeatValue):null;
        //    Created = tag.Created;
        //}
    }

}
