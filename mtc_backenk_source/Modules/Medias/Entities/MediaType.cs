using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Medias.Entities
{
    [Table("mtc_media_type_tbl")]
    public class MediaType
    {
        [Key]
        [Column("type_id")]
        public string TypeId { get; set; } = Guid.NewGuid().ToString();
        [Column("type_name")]
        public string TypeName { get; set; }
        [Column("type_code")]
        public string TypeCode { get; set; }
        [Column("type_icon")]
        public string TypeIcon { get; set; } = null;
        [Column("type_comment")]
        public string TypeComment { get; set; } = null;
        [Column("type_status")]
        [EnumDataType(typeof(MediaTypeStatusEnum))]
        public MediaTypeStatusEnum TypeStatus { get; set; } = MediaTypeStatusEnum.USERD;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class MediaTypeResponse
    {
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public string TypeIcon { get; set; } = null;
        public string TypeComment { get; set; } = null;
        public MediaTypeStatusEnum TypeStatus { get; set; }

        public MediaTypeResponse(MediaType mediaType, string url)
        {
            TypeId = mediaType.TypeId;
            TypeName = mediaType.TypeName;
            TypeCode = mediaType.TypeCode;
            TypeIcon = mediaType.TypeIcon == null ? null : GeneralHelper.UrlCombine(url, mediaType.TypeIcon);
            TypeStatus = mediaType.TypeStatus;
            TypeComment = mediaType.TypeComment;
        }
    }


    public enum MediaTypeStatusEnum
    {
        USERD = 1,
        DELETED = 0
    }


}

