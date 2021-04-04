using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Entities
{
    [Table("wh_destroy")]
    public class Destroy
    {
        [Key]
        [Column("destroy_id")]
        public int DestroyId { get; set; }
        [Column("destroy_code")]
        public string DestroyCode { get; set; }
        [Column("destroy_request_date")]
        public DateTime DestroyRequestDate { get; set; } // ngày đề xuất (ngày hiện tại)
        [Column("destroy_date")]
        public DateTime? DestroyDate { get; set; } // Ngày hủy (người dùng nhập)
        [Column("destroy_status")]
        [EnumDataType(typeof(DestroyStatus))]
        public DestroyStatus DestroyStatus { get; set; } =  DestroyStatus.UNCONFIMRED;
        [Column("destroy_user")]
        public string DestroyUser { get; set; }
        public List<DestroyDetail> DestroyDetails { get; set; }
    }


    public enum DestroyStatus
    {
        UNCONFIMRED = 0,
        CONFIRMED = 1,

    }

}
