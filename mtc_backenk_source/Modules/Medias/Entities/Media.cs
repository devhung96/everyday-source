using Newtonsoft.Json;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Medias.Entities
{
    [Table("mtc_media_tbl")]
    public class Media
    {
        [Key]
        [Column("media_id")]
        public string MediaId { get; set; } = Guid.NewGuid().ToString();
        [Column("media_name")]
        public string MediaName { get; set; }
        [Column("media_url")]
        public string MediaUrl { get; set; }
        [Column("media_url_optional")]
        public string MediaUrlOptional { get; set; }
        [Column("media_status")]
        [EnumDataType(typeof(MediaStatusEnum))]
        public MediaStatusEnum MediaStatus { get; set; } = MediaStatusEnum.NotConfirm;
        [Column("media_comment")]
        public string MediaComment { get; set; }
        [Column("media_type_code")]
        public string MediaTypeCode { get; set; }
        [Column("media_size")]
        public string MediaSize { get; set; }
        [Column("media_duration")]
        public string MediaDuration { get; set; }
        [Column("media_md5")]
        public string MediaMd5 { get; set; }
        [Column("media_type_id")]
        public string TypeId { get; set; }
        [Column("media_user_id")]
        public string UserId { get; set; }
        [Column("created_at")]
        public DateTime CreateAt { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        public virtual User User { get; set; }
        public List<MediaGroup> MediaGroups { get; set; }
        [Column("media_extend")]
        public string MediaExtend { get; set; } 
        [NotMapped]
        public object MediaExtendParse
        {
            get
            {
                if(String.IsNullOrEmpty(MediaExtend)) return null;
                return JsonConvert.DeserializeObject(MediaExtend);
            }
        }
    }

    public enum MediaStatusEnum
    {
        Confirm = 1,
        NotConfirm = 0,
        Reject = -1
    }

    
}
