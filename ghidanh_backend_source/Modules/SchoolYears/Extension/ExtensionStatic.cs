using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.App.Databases;
using Project.App.Helpers;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.Semesters.Entities;
using Project.Modules.SubjectGroups.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Classes.Entities.Class;

namespace Project.Modules.SchoolYears.Extension
{
    public static class CheckList<T> where T : class
    {
        //public static bool FieldExists(this RequestTable requestTable, string sortField)
        //public bool FieldExists(this List<T> listEntity, string sortField)
        //{
        //    //var fields = typeof(T)
        //    return true;
        //}
        //public void FieldExist(ThisExpressionSyntax )
        //{

        //}
    }
    public static class ExtensionStatic
    {
        public static IConfiguration Configuration;

        public static IConfiguration SetUtilsProviderConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            Configuration = configuration;
            return Configuration;
        }
        private static readonly List<string> vnstring = new List<string>
        {
            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };
        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };
        public static string RemoveSign4VietnameseString(this string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            str = str.Replace("quốc", "quoc");
            str = str.Replace("khánh", "khanh");
            return str;
        }
        public static string RemoveUnicode(this string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ", "khánh" , "quốc","́" };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y","khanh","quoc" , ""};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }
        public static bool FieldExists<T>(this List<T> listEntity, string sortField)
        {
            List<string> fields = typeof(T).GetProperties()
                        .Select(property => property.Name)
                        .ToList();
            if (sortField is null)
            {
                Console.WriteLine("SortFieldNull");
                return false;
            }
            return fields.Any(x => x.ToLower().Equals(sortField.ToLower()));
        }

        public static List<SubjectGroup> FilterClass(this List<SubjectGroup> subjectGroups, List<ClassSchedule> classSchedules)
        {
            return subjectGroups
                .Where(x =>
                    x.Subjects.Any(z => z.Classes.Count > 0) &&
                    x.Subjects.Any(c =>
                        c.Classes.Any(v =>
                            v.Admission == STATUS_OPEN.OPEN
                        )
                    ) &&
                    x.Subjects.Any(s => classSchedules.Any(v => v.SubjectId.Equals(s.SubjectId)))
                )
                .ToList();
        }

        public static bool CheckConditionSemester(this Semester semester, bool flagInsert = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            var MariaDbContext = new MariaDBContext(optionsBuilder.Options);
            List<Semester> semesters = MariaDbContext.Semesters.Where(x => x.SchoolYearId.Equals(semester.SchoolYearId)).ToList();
            if (!flagInsert)
            {
                if (semesters.Any(x => x.TimeEnd > semester.TimeStart))
                {
                    return false;
                }
                return true;
            }
            else
            {
                semesters = semesters.Where(x => !x.SemesterId.Contains(semester.SemesterId)).ToList();
                if (semesters.Any(x => x.TimeStart < semester.TimeStart && x.TimeEnd > semester.TimeStart))
                {
                    return false;
                }
                if (semesters.Any(x => x.TimeStart < semester.TimeEnd && x.TimeEnd > semester.TimeEnd))
                {
                    return false;
                }

                return true;
            }
        }

        public static bool CheckConditionSchoolYear(this Entities.SchoolYear schoolYears, bool flagInsert = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            var MariaDbContext = new MariaDBContext(optionsBuilder.Options);
            List<SchoolYear> ListSchoolYears = MariaDbContext
                                                        .SchoolYears
                                                        //.Where(x => x.SchoolYearId.Equals(schoolYears.SchoolYearId))
                                                        .ToList();

            if (!flagInsert)
            {
                //if (ListSchoolYears.Any(x => x.TimeEnd > schoolYears.TimeStart))
                if (ListSchoolYears.Any(x => x.TimeStart > schoolYears.TimeStart) || ListSchoolYears.Any(x => x.TimeEnd > schoolYears.TimeStart))
                {
                    return false;
                }
                return true;
            }
            else
            {
                ListSchoolYears = ListSchoolYears.Where(x => !x.SchoolYearId.Contains(schoolYears.SchoolYearId)).ToList();
                if (ListSchoolYears.Any(x => x.TimeStart < schoolYears.TimeStart && x.TimeEnd > schoolYears.TimeStart))
                {
                    return false;
                }
                if (ListSchoolYears.Any(x => x.TimeStart < schoolYears.TimeEnd && x.TimeEnd > schoolYears.TimeEnd))
                {
                    return false;
                }

                //check ton tai hoc ki 
                return true;
            }
        }

        public static bool TypeToken(this string Type)
        {
            if (Type != null && (Type.ToLower() == "admin" || Type.ToLower() == "lecture"))
            {
                return true;
            }
            return false;
        }
    }
}
