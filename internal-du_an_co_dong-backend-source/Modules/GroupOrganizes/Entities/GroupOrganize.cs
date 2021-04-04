using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.GroupOrganizes.Entities
{
  
    [Table("shareholder_group_organize")]
    public class GroupOrganize
    {
        [Key]
        [Column("group_organize_id")]
        public int GroupOrganizeId { get; set; }
        [Column("group_organize_name")]
        public string GroupOrganizeName { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("organize_id")]
        public string OrganizeId { get; set; }
        [Column("group_organize_created_at")]
        public DateTime? GroupOrganizeCreatedAt { get; set; }
        [Column("group_organize_order")]
        public int GroupOrganizeOrder { get; set; }
        public GroupOrganize() { }

    }
}
