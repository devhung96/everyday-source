using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Permissions.Entities
{
    [Table("shareholder_permission_type")]
    public class PermissionType
    {
        [Key]
        [Column("permission_type_id")]
        public int PermissionTypeId { get; set; }
        [Column("permission_type_name")]
        public string PermissionTypeName { get; set; }
        [Column("permission_type_code")]
        public string PermissionTypeCode { get; set; }
    }
}
