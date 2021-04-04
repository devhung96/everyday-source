using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Entities
{
    [Table("hdbank_ac_register_detect_details")]
    public class RegisterDetectDetail
    {
        [Key]
        [Column("rg_detect_detail_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RgDetectDetailId { get; set; }



        [Column("rg_detect_id")]
        public string RegisterDetectId { get; set; } 

        /// <summary>
        /// Thời gian bắt đầu vào.
        /// </summary>
        [Column("rg_detect_detail_date_begin")]
        public DateTime RgDectectDetailDateBegin { get; set; }


        /// <summary>
        /// Thời gian hết hạn.
        /// </summary>
        [Column("rg_detect_detail_date_end")]
        public DateTime? RgDectectDetailDateEnd { get; set; }


        /// <summary>
        /// Loại repeat của lịch
        /// </summary>
        [Column("rg_detect_detail_repeat")]
        [EnumDataType(typeof(RepeatType))]
        public RepeatType RgDectectDetailRepeat { get; set; }


        /// <summary>
        /// Giá trị tương ứng với loại repeat
        /// </summary>
        [Column("rg_detect_detail_repeat_value")]
        public string RgDectectDetailRepeatValue { get; set; }

        [NotMapped]
        public List<string> RgDectectDetailRepeatValueData { 
            get
            {
                if (String.IsNullOrEmpty(RgDectectDetailRepeatValue)) return new List<string>();
                return JsonConvert.DeserializeObject<List<string>>(RgDectectDetailRepeatValue);
            }
        }



        /// <summary>
        /// Thời gian bắt đầu trong ngày mà user có quyền truy cập vào.
        /// </summary>
        [Column("rg_detect_detail_time_begin")]
        public TimeSpan RgDectectDetailTimeBegin { get; set; }


        /// <summary>
        /// Thời gian hết hạn trong ngày mà user có quyền truy cập vào.
        /// </summary>
        [Column("rg_detect_detail_time_end")]
        public TimeSpan RgDectectDetailTimeEnd { get; set; } 



        [Column("rg_detect_detail_created_at")]
        public DateTime RgDectectDetailCreatedAt { get; set; } = DateTime.UtcNow;

        //[NotMapped]
        public RegisterDetect RegisterDetect { get; set; }
    }


    public enum RepeatType
    {
        NonRepeat = 0,
        RepeatDay = 1,
        RepeatWeek = 2,
        RepeatMonth = 3,
        RepeatYear = 4
    }
}
