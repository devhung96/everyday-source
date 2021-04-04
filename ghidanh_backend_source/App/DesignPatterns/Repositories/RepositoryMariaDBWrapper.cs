using Microsoft.EntityFrameworkCore.Storage;
using Project.App.Databases;
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

namespace Project.App.DesignPatterns.Repositories
{
    public interface IRepositoryMariaWrapper
    {
        IRepositoryRoot<Permission> Permissions { get; }
        IRepositoryRoot<Module> Modules { get; }
        IRepositoryRoot<Receipt> Receipts { get; }
        IRepositoryRoot<Class> Classes { get; }
        IRepositoryRoot<Lecturer>Lecturers  { get; }
        IRepositoryRoot<LecturerSubject> LecturerSubjects { get; }
        IRepositoryRoot<User> Users { get; }
        IRepositoryRoot<Account> Accounts { get; }
        IRepositoryRoot<AccountPermission> AccountPermissions { get; }
        IRepositoryRoot<Group> Groups { get; }
        IRepositoryRoot<GroupPermission> GroupPermissions { get; }
        IRepositoryRoot<RegistrationStudy> RegistrationStudies { get; }
        IRepositoryRoot<Student> Students { get; }
        IRepositoryRoot<Slot> Slots { get; }
        IRepositoryRoot<SubjectGroup> SubjectGroups { get; }
        IRepositoryRoot<Subject> Subjects { get; }
        IRepositoryRoot<ScoreType> ScoreTypes { get; }
        void SaveChanges();
        IDbContextTransaction BeginTransaction();
        IRepositoryRoot<Course> Courses { get; }
        IRepositoryRoot<SchoolYear> SchoolYears { get; }
        IRepositoryRoot<ClassSchedule> ClassSchedules { get; }
        IRepositoryRoot<ScoreTypeSubject> ScoreTypeSubjects { get; }
        IRepositoryRoot<Semester> Semesters { get; }
        IRepositoryRoot<CourseInSchoolYear> CourseInSchoolYears { get; }
        IRepositoryRoot<CourseSubject> CourseSubjects { get; }
        IRepositoryRoot<ReportClosing> ReportClosings { get; }
        IRepositoryRoot<Holiday> Holidays { get; }
        IRepositoryRoot<Contact> Contacts { get; }
        IRepositoryRoot<MoneyType> MoneyTypes { get; }
        IRepositoryRoot<Surcharge> Surcharges { get; }
        IRepositoryRoot<EmployeeSalary> EmployeeSalaries { get; }
    }
    public class RepositoryMariaDBWrapper : IRepositoryMariaWrapper
    {
        private readonly MariaDBContext DbContext;
        private RepositoryMariaBase<Permission> permissionService;
        private RepositoryMariaBase<Module> modules;
        private RepositoryMariaBase<Receipt> receipt;
        private RepositoryMariaBase<Class> classService;
        private RepositoryMariaBase<RegistrationStudy> registrationStudies;
        private RepositoryMariaBase<Student> studentService; 
        private RepositoryMariaBase<User> userService;
        private RepositoryMariaBase<Account>accountService;
        private RepositoryMariaBase<AccountPermission>accountPermissionService;     
        private RepositoryMariaBase<Group>group;
        private RepositoryMariaBase<GroupPermission> groupPermission;
        private RepositoryMariaBase<Lecturer>lecturerService;
        private RepositoryMariaBase<LecturerSubject> lecturerSubjectService;
        private RepositoryMariaBase<Slot> slotService;
        private RepositoryMariaBase<SubjectGroup> subjectGroupService;
        private RepositoryMariaBase<Subject> subjectService;
        private RepositoryMariaBase<ScoreType> scoreTypes;
        private RepositoryMariaBase<Course> course;
        private RepositoryMariaBase<SchoolYear> schoolYears;
        private RepositoryMariaBase<ClassSchedule> classSchedules;
        private RepositoryMariaBase<ScoreTypeSubject> scoreTypeSubject;
        private RepositoryMariaBase<Semester> semesters;
        private RepositoryMariaBase<CourseInSchoolYear> courseInSchoolYears;
        private RepositoryMariaBase<CourseSubject> courseSubjects;
        private RepositoryMariaBase<ReportClosing> reportClosings;
        private RepositoryMariaBase<Holiday> holidays;
        private RepositoryMariaBase<Contact> contacts;
        private RepositoryMariaBase<MoneyType> moneyTypeService;
        private RepositoryMariaBase<Surcharge> surcharges;
        private RepositoryMariaBase<EmployeeSalary> employeeSalaries;

        public RepositoryMariaDBWrapper(MariaDBContext dbContext)
        {
            DbContext = dbContext;
        }
        public IRepositoryRoot<Contact> Contacts
        {
            get
            {
                return contacts ??= new RepositoryMariaBase<Contact>(DbContext);
            }
        }
        public IRepositoryRoot<Receipt> Receipts
        {
            get
            {
                return receipt ??= new RepositoryMariaBase<Receipt>(DbContext);
            }
        }  
        public IRepositoryRoot<Module> Modules
        {
            get
            {
                return modules ??= new RepositoryMariaBase<Module>(DbContext);
            }
        }
        public IRepositoryRoot<CourseSubject> CourseSubjects
        {
            get
            {
                return courseSubjects ??= new RepositoryMariaBase<CourseSubject>(DbContext);
            }
        }
        public IRepositoryRoot<Holiday> Holidays
        {
            get
            {
                return holidays ??= new RepositoryMariaBase<Holiday>(DbContext);
            }
        }
        public IRepositoryRoot<ScoreTypeSubject> ScoreTypeSubjects
        {
            get
            {
                return scoreTypeSubject ??= new RepositoryMariaBase<ScoreTypeSubject>(DbContext);
            }
        }

        public IRepositoryRoot<Semester> Semesters
        {
            get
            {
                return semesters ??= new RepositoryMariaBase<Semester>(DbContext);
            }
        }

        public IRepositoryRoot<Class> Classes
        {
            get
            {
                return classService ??= new RepositoryMariaBase<Class>(DbContext);
            }
        }
        public IRepositoryRoot<ScoreType> ScoreTypes
        {
            get
            {
                return scoreTypes ??= new RepositoryMariaBase<ScoreType>(DbContext);
            }
        }

        public IRepositoryRoot<Lecturer> Lecturers
        {
            get
            {
                return lecturerService ??= new RepositoryMariaBase<Lecturer>(DbContext);
            }
        }  
        public IRepositoryRoot<LecturerSubject> LecturerSubjects
        {
            get
            {
                return lecturerSubjectService ??= new RepositoryMariaBase<LecturerSubject>(DbContext);
            }
        }

        public IRepositoryRoot<Student> Students
        {
            get
            {
                return studentService ??= new RepositoryMariaBase<Student>(DbContext);
            }
        }

        public IRepositoryRoot<User> Users
        {
            get
            {
                return userService ??= new RepositoryMariaBase<User>(DbContext);
            }
        }
        public IRepositoryRoot<Account> Accounts
        {
            get
            {
                return accountService ??= new RepositoryMariaBase<Account>(DbContext);
            }
        } 
        public IRepositoryRoot<AccountPermission> AccountPermissions
        {
            get
            {
                return accountPermissionService ??= new RepositoryMariaBase<AccountPermission>(DbContext);
            }
        }   
        public IRepositoryRoot<Group> Groups
        {
            get
            {
                return group ??= new RepositoryMariaBase<Group>(DbContext);
            }
        }  
        public IRepositoryRoot<GroupPermission> GroupPermissions
        {
            get
            {
                return groupPermission ??= new RepositoryMariaBase<GroupPermission>(DbContext);
            }
        }  
        public IRepositoryRoot<RegistrationStudy> RegistrationStudies
        {
            get
            {
                return registrationStudies ??= new RepositoryMariaBase<RegistrationStudy>(DbContext);
            }
        }

        public IRepositoryRoot<Slot> Slots
        {
            get
            {
                return slotService ??= new RepositoryMariaBase<Slot>(DbContext);
            }
        }

        public IRepositoryRoot<SubjectGroup> SubjectGroups
        {
            get
            {
                return subjectGroupService ??= new RepositoryMariaBase<SubjectGroup>(DbContext);
            }
        }

        public IRepositoryRoot<Subject> Subjects
        {
            get
            {
                return subjectService ??= new RepositoryMariaBase<Subject>(DbContext);
            }
        }

        public IRepositoryRoot<Course> Courses
        {
            get
            {
                return course ??= new RepositoryMariaBase<Course>(DbContext);
            }
        }
        public IRepositoryRoot<SchoolYear> SchoolYears
        {
            get
            {
                return schoolYears ??= new RepositoryMariaBase<SchoolYear>(DbContext);
            }
        }
        public IRepositoryRoot<ClassSchedule> ClassSchedules
        {
            get
            {
                return classSchedules ??= new RepositoryMariaBase<ClassSchedule>(DbContext);
            }
        }

        public IRepositoryRoot<CourseInSchoolYear> CourseInSchoolYears
        {
            get
            {
                return courseInSchoolYears ??= new RepositoryMariaBase<CourseInSchoolYear>(DbContext);
            }
        }

        public IRepositoryRoot<ReportClosing> ReportClosings
        {
            get
            {
                return reportClosings ??= new RepositoryMariaBase<ReportClosing>(DbContext);
            }
        }  
        public IRepositoryRoot<Permission> Permissions
        {
            get
            {
                return permissionService ??= new RepositoryMariaBase<Permission>(DbContext);
            }
        }

        public IRepositoryRoot<MoneyType> MoneyTypes
        {
            get
            {
                return moneyTypeService ??= new RepositoryMariaBase<MoneyType>(DbContext);
            }
        }

        public IRepositoryRoot<Surcharge> Surcharges
        {
            get
            {
                return surcharges ??= new RepositoryMariaBase<Surcharge>(DbContext);
            }
        }

        public IRepositoryRoot<EmployeeSalary> EmployeeSalaries 
        {
            get
            {
                return employeeSalaries ??= new RepositoryMariaBase<EmployeeSalary>(DbContext);
            }
        }
        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
        public IDbContextTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction();
        }
    }
}
