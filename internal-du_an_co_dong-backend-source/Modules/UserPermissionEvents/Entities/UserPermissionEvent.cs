using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UserPermissionEvents.Entities
{
    [Table("shareholder_user_permission_event")]
    public class UserPermissionEvent
    {
        [Key]
        [Column("user_event_permission_id")]
        public int UserPerEventId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("event_id")]
        public string EventId { get; set; }
        [Column("permission_id")]
        public int PermissionId { get; set; }
        [Column("permission_code")]
        public string PermissionCode { get;set; }
    }
}
