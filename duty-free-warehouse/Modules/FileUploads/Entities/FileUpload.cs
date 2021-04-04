using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.FileUploads.Entities
{
    [Table("wh_file_upload")]
    public class FileUpload
    {
        [Key]
        [Column("file_id")]
        public int FileID { get; set; }
        [Column("file_type")]
        [EnumDataType(typeof(FileTye))]
        public FileTye FileType { get; set; }
        [Column("user_id")]
        public int UserID { get; set; }
        [Column("file_name")]
        public string FileName { get; set; }
        [Column("file_path")]
        public string FilePath { get; set; }
        [Column("file_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }

    }

    public enum FileTye
    {
        Seal = 1,
        Sell = 2
    }

}
