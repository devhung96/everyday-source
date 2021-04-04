using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Entities
{
    [Table("shareholder_organize")]
    public class Organize
    {
        [Key]
        [Column("organize_id")]
        public string OrganizeId { get; set; } = Guid.NewGuid().ToString();


        [Column("organize_logo_url")]
        public string OrganizeLogoUrl { get; set; }


        [Column("organize_name")]
        public string OrganizeName { get; set; }

        [Column("organize_address")]
        public string OrganizeAddress { get; set; }

        [Column("organize_introduce")]
        public string OrganizeIntroduce { get; set; }

        [Column("organize_code_ck")]
        public string OrganizeCodeCk { get; set; }

        [Column("organize_stocks")]
        public int OrganizeStocks { get; set; }


        [Column("organize_email")]
        public string OrganizeEmail { get; set; }

        [Column("organize_website_url")]
        public string OrganizeWebsiteURL { get; set; }

        [Column("organize_landing_page_url")]
        public string OrganizeLandingPageUrl { get; set; }

        [Column("organize_facebook_url")]
        public string OrganizeFacebookUrl { get; set; }

        [Column("organize_setting")]
        public string OrganizeSetting { get; set; }


        [Column("user_id")]
        public string UserId { get; set; }


        [NotMapped]
        public string UserAdmin { get; set; }

        [NotMapped]
        public object UserAdminInfo { get; set; }


        [Column("organize_code_dn")]
        public string OrganizeCodeDn { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }


        [Column("organize_status")]
        [EnumDataType(typeof(ORGANIZE_STATUS))]
        public ORGANIZE_STATUS OrganizeStatus { get; set; } = ORGANIZE_STATUS.ACTIVED;

        [NotMapped]
        public DateTime EventTimeBegin { get; set; }

        [NotMapped]
        public DateTime EventTimeEnd { get; set; }

        [NotMapped]
        public string Owner { get; set; }

        [NotMapped]
        public string EventLogoUrl { get; set; }

        [NotMapped]
        public string EventName { get; set; }

        [NotMapped]
        public int NumberOfShareholders { get; set; }

        [NotMapped]
        public int EventStock { get; set; }




    }

    public enum ORGANIZE_STATUS
    {
        ACTIVED =1 ,
        DELETED = 0
    } 
    
    //public enum ORGANIZE_FLAG
    //{
    //    CREATED = 0,
    //    BEGIN = 1,
    //    END = 2
    //}
}
