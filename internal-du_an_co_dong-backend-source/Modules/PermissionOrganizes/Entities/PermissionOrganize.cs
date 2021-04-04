using Project.Modules.Permissions.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Entities
{

    [Table("shareholder_permission_organize")]
    public class PermissionOrganize
    {
        [Key]
        [Column("permission_organize_id")]
        public int PermissionOrganizeId { get; set; }//
        [Column("permission_organize_code")]
        public string PermissionOrganizeCode { get; set; }
        [Column("permission_organize_name")]
        public string PermissionOrganizeName { get; set; }
       [ForeignKey("fk_function_organize")]
        [Column("function_organize_id")]
        public int FunctionOrganizeId { get; set; }

        [Column("permission_type_id")]
        public int PermissionTypeId { get; set; }

       public PermissionType PermissionType { get; set; }

        [NotMapped]
        public bool PermissionFlag { get; set; } = false;
        public FunctionOrganize FunctionOrganize { get; set; }

    }

}
