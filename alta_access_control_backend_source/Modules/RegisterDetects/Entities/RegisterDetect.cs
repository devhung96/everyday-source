using Newtonsoft.Json.Linq;
using Project.Modules.Tickets.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Entities
{
    [Table("hdbank_ac_register_detects")]
    public class RegisterDetect
    {
        [Key]
        [Column("rg_detect_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RegisterDetectId { get; set; } 
        /// <summary>
        /// User id của hệ thống khác 
        /// </summary>
        [Column("rg_detect_user_id")]
        public string RgDectectUserId { get; set; }

        /// <summary>
        /// Key : Để dectect 
        /// = Face ID
        /// = Card ID
        /// </summary>
        [Column("rg_detect_key")]
        public string RgDectectKey { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column("tag_code")]
        public string TagCode { get; set; }

        /// <summary>
        /// Loại xác thực : Face, CardID
        /// </summary>
        [Column("mode_id")]
        public string ModeId { get; set; }


        /// <summary>
        /// Device mà user đc phép truy cập vào
        /// </summary>
        [Column("ticket_type_id")]
        public string TicketTypeId { get; set; }


        /// <summary>
        /// Lưu dữ liệu client muốn thêm
        /// </summary>
        [Column("rg_detect_extension")]
        public string RgDectectExtension { get; set; }


        [Column("rg_detect_created_at")]
        public DateTime RgDectectCreatedAt { get; set; } = DateTime.UtcNow;

        [Column("rg_detect_updated_at")]
        public DateTime? RgDectectUpdatedAt { get; set; }
        [NotMapped]
        public string UserName
        {
            get
            {
                if (String.IsNullOrEmpty(RgDectectExtension)) return null;
                return (JToken.Parse(RgDectectExtension)["UserLastName"].ToString()+" "+ JToken.Parse(RgDectectExtension)["UserFirstName"].ToString());
            }
        }
    }
}
