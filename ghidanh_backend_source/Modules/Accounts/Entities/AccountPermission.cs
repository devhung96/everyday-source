using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Accounts.Entities
{
    [Table("rc_account_permissions")]
    public class AccountPermission
    {
        [Key]
        [Column("account_permission_id")]
        public string AccountPermissionId { get; set; } = Guid.NewGuid().ToString();
        [Column("account_id")]
        public string AccountId { get; set; }
        [Column("permission_code")]
        public string PermissionCode { get; set; }
        public AccountPermission() { }
        public AccountPermission(string id, string permissionCode)
        {
            AccountId = id;
            PermissionCode = permissionCode;
        }
    }
}
