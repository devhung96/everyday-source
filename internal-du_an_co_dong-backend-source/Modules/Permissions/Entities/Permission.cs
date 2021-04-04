using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Permissions.Entities
{

    [Table("shareholder_permission")]
    public class PermissionOrganize
    {
        [Key]
        [Column("permission_id")]
        public int PermissionId { get; set; }
        [Column("permission_code")]
        [MaxLength(255)]
        public string PermissionCode { get; set; }
        [MaxLength(255)]
        [Column("permisson_name")]
        public string PermissionName { get; set; }
        [ForeignKey("fk_function")]
        [Column("function_id")]
        public int FunctionId { get; set; }

        [Column("permission_type_id")]
        public int PermissionTypeId { get; set; }

        public PermissionType PermissionType { get; set; }

        [NotMapped]
        public bool PermissionFlag { get; set; } = false;

    }

}
