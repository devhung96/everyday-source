using Project.Modules.Lecturers.Entities;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SubjectGroups.Entities
{
    [Table("rc_subject_groups")]
    public class SubjectGroup
    {
        [Key]
        [Column("subject_group_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SubjectGroupId { get; set; }
        [Column("subject_group_name")]
        public string SubjectGroupName { get; set; }
        [Column("subject_group_created")]
        public DateTime SubjectGroupCreatedAt { get; set; } = DateTime.UtcNow;
        public List<Subject> Subjects { get; set; }
    }
}
