using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionGroups.Entities
{
  
        [Table("shareholder_permission_group")]
        public class PermissionGroup
        {
            [Key]
            [Column("permission_group_id")]
            public int PermissionGroupId { get; set; }
            [Column("group_id")]
            public int GroupId { get; set; }
            [Column("permission_id")]
            public int PermissionId { get; set; }
            [Column("permission_code")]
            public string PermissionCode { get; set; }
            public PermissionGroup()
            {
            }
        }

}
