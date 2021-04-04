using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Lecturers.Entities
{

    /// <summary>
    /// Luong nhan vien
    /// </summary>
    [Table("rc_employee_salary")]
    public class EmployeeSalary
    {
        [Key]
        [Column("employee_salary_id")]
        [StringLength(36)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EmployeeSalaryId { get; set; }

        [Column("course_id")]
        public string CourseId { get; set; }

        [Column("lecture_id")]
        public string LectureId { get; set; }

        /// <summary>
        /// Phiếu số lương
        /// </summary>
        [Column("employee_salary_number")]
        public int EmployeeSalaryNumber { get; set; }


        [Column("employee_salary_note")]
        public string EmployeeSalaryNote { get; set; }


        /// <summary>
        /// Lương cơ bản
        /// </summary>
        [Column("employee_salary_basic")]
        public double EmployeeSalaryBasic { get; set; }

        /// <summary>
        /// Phần trăm phụ cấp ưu đãi
        /// </summary>
        [Column("employee_salary_subsidies_percent")]
        public double EmployeeSalarySubsidiesPercent { get; set; }


        /// <summary>
        /// Phụ cấp khác
        /// </summary>
        [Column("employee_salary_subsidies_other")]
        public double EmployeeSalarySubsidiesOther { get; set; }
        [Column("employee_salary_subsidies_other_name")]
        public string EmployeeSalarySubsidiesOtherName { get; set; }

        /// <summary>
        /// Lương thực nhận chưa cộng phụ cấp
        /// </summary>
        [Column("employee_salary_real")]
        public double EmployeeSalaryReal { get; set; }

        /// <summary>
        /// Lương sau cùng
        /// </summary>
        [Column("employee_salary_total")]
        public double EmployeeSalaryTotal { get; set; }


        [Column("employee_salary_confirm")]
        [EnumDataType(typeof(EmployeeSalaryConfirm))]
        public EmployeeSalaryConfirm EmployeeSalaryConfirm { get; set; } = EmployeeSalaryConfirm.UnConfirm;


        [Column("employee_salary_created_at")]
        public DateTime EmployeeSalaryCreatedAt { get; set; } = DateTime.UtcNow;


        [Column("employee_salary_updated_at")]
        public DateTime EmployeeSalaryUpdatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public bool CheckUpdate { get; set; }

    }

    public enum EmployeeSalaryConfirm
    {
        Confirm = 1,
        UnConfirm = 0
    }
}
