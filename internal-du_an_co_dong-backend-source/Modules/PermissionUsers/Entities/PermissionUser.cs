using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.PermissonUsers
{
    [Table("shareholder_permission_user")]
    public class PermissionUser
    {
        [Key]
        [Column("permission_user_id")]
        public int PerssionUserId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("permission_code")]
        public string PermissionCode { get; set; }
        public PermissionUser()
        {
        }

        public enum PERMISSION_DEFAULT
        {
            ADMIN,
            SUPER,
            CLIENT
        }
    }
}
