using Newtonsoft.Json;
using Project.Modules.Organizes.Entities;
//using Project.Modules.Surveys.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("shareholder_user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        [Column("user_shareholder_code")]
        public string ShareholderCode { get; set; }
        [JsonIgnore]
        [Column("user_password")]
        public string UserPassword { get; set; }
        [Column("user_password_system")]
        public string PasswordSystem{ get; set; }
        [Column("user_email")]
        public string UserEmail { get; set; }
        [Column("user_full_name")]
        public string FullName { get; set; }
        [Column("user_identity_card")]
        public string IdentityCard { get; set; }
        [Column("user_place_of_issue")]
        public string PlaceOfIssue { get; set; }
        [Column("user_issue_date")]
        public DateTime IssueDate { get; set; }
        [Column("user_phone_number")]
        public string PhoneNumber { get; set; }
        [Column("user_image")]
        public string UserImage { get; set; }
        [Column("organize_id")]// số cổ phần
        public string OrganizeId { get; set; }
        [Column("user_stock_code")]
        public string StockCode { get; set; } //Mã chứng khoán
        [Column("user_status")]
        public USER_STATUS UserStatus { get; set; } = USER_STATUS.ACTIVE;
        //[JsonIgnore]
        //[Column("user_deleted")]
        //public DELETE_STATUS UserDelete { get; set; } = DELETE_STATUS.EXIST;
        [JsonIgnore]
        [Column("user_salt")]
        public string UserSalt { get; set; }
        [Column("user_created_at")]
        public DateTime UserCreatedAt { get; set; } = DateTime.Now;
        [Column("group_organize_id")]
        public int ? GroupOrganizeId { get; set; }

        [Column("send_mail")]
        public SEND_STATUS SendMail { get; set; }
        [NotMapped]
        public int UserStock { get; set; }
        [NotMapped]
        public int UserInviteStatus { get; set; }
        //public Organize Organize { get; set; }

        #region  ENUM
        public enum USER_STATUS
        {
            ACTIVE = 0, 
            BLOCK = 1
        }

        public enum SEND_STATUS
        {
            DA = 1,
            CHUA = 0
        }
        public enum DELETE_STATUS
        {
            EXIST = 0 ,
            DELETED = 1
        }
        public enum USER_TYPE
        {
            SUPER = 2 ,
            ADMIN = 1,
            CLIENT =0
        }
        #endregion


    }
}
