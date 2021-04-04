using Project.Modules.PermissionGroups.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Entities
{
    [Table("shareholder_group")]
    public class Group
    {
        [Column("group_id")]
        public int GroupID { get; set; }
        [Column("group_name")]
        public string GroupName { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("event_id")]
        public string EventId { get; set; }
        [Column("group_created_at")]
        public DateTime? GroupCreatedAt { get; set; }
        [Column("group_order")]
        public int GroupOrder { get; set; }
        public Group() { }
      
        //[NotMapped]
        //public double? ConvertGroupExpireDate { get {  return GroupExpireDate?.TotalSeconds; }}
    }
}
