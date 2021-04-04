using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Project.Modules.Events.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Users.Entities.User;

namespace Project.Modules.Organizes.Entities
{
    [Table("shareholder_event_user_group")]
    public class EventUser
    {
        [Key]
        [Column("organize_detail_id")]
        public int EventUserId { get; set; }
        [Column("event_id")]
        public string EventId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("group_id")]
        public int GroupId { get; set; }
        [Column("count_send_invitation")]
        public int SendInvitation { get; set; }
        [Column("stock_user_event")]
        public int UserStock { get; set; }
        [Column("user_block")]
        public STATUS UserActive { get; set; } = STATUS.ACTIVE;
        [JsonIgnore]
        [Column("user_password")]
        public string UserPassword { get; set; }
        [JsonIgnore]
        [Column("user_salt")]
        public string UserSalt { get; set; }
       // [JsonIgnore]
        [Column("user_password_system")]
        public string PasswordSystem { get; set; }
        [Column("send_mail_cms")]
        public SEND_STATUS SendMailCMS { get; set; } = SEND_STATUS.CHUA;
        public enum STATUS
        {
            ACTIVE = 1,
            BLOCK = 0
        }

      //  public Organize Organize { get; set; }
        public Event Event { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }

        [Column("user_login_status")]
        [EnumDataType(typeof(USER_LOGIN_STATUS))]
        public USER_LOGIN_STATUS UserLoginStatus { get; set; } = USER_LOGIN_STATUS.OFF;

        /// <summary>
        /// 4 Status : Don't Send, Sent, Deny, Pending
        /// </summary>
        [Column("user_invite_status")]
        public int UserInviteStatus { get; set; }

        [Column("user_latch")]
        [EnumDataType(typeof(USER_LATCH))]
        public USER_LATCH UserLatch { get; set; } = USER_LATCH.OFF;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("user_login_at")]
        public DateTime? LoginAt { get; set; }
        [Column("user_token")]
        public string UserToken { get; set; }
        [NotMapped]
        public int StockReceive { get; set; }



    }


    public enum USER_LATCH
    {
        ON = 1,
        OFF = 0
    }
    public enum USER_LOGIN_STATUS
    {
        ON = 1,
        OFF = 0
    }
}
