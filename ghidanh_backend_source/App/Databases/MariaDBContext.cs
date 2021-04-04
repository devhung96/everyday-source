using Microsoft.EntityFrameworkCore;
using Project.Modules.Accounts.Entities;
using Project.Modules.Classes.Entities;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.Contacts.Entities;
using Project.Modules.Courses.Entities;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Holidays.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.MoneyTypes.Entities;
using Project.Modules.Receipts.Entities;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.Scores.Entities;
using Project.Modules.ScoreTypes.Entities;
using Project.Modules.Semesters.Entities;
using Project.Modules.Slots.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.SubjectGroups.Entities;
using Project.Modules.Subjects.Entities;
using Project.Modules.Users.Entities;

namespace Project.App.Databases
{
    public class MariaDBContext : DbContext
    {
        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options)
        {

        }

       public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<LecturerSubject> LecturerSubjects { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountPermission> AccountPermissions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }
        public DbSet<RegistrationStudy> RegistrationStudies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectGroup> SubjectGroups { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<ScoreType> ScoreTypes { get; set; }
        public DbSet<ScoreTypeSubject> ScoreTypeSubjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<CourseInSchoolYear> CourseInSchoolYears { get; set; }
        public DbSet<CourseSubject> CourseSubjects { get; set; }
        public DbSet<ReportClosing> ReportClosings { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<MoneyType> MoneyTypes { get; set; }
        public DbSet<Surcharge> Surcharges { get; set; }
        public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasQueryFilter(p => p.Enable);
            modelBuilder.Entity<SlotInSchoolYear>().HasKey(s => new { s.SlotId, s.SchoolYearId });
        }
    }
}
