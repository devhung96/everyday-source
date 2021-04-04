using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Users.Services
{
    public interface IDepartmentService
    {
        (object data, string message) CreateDepartment(AddDepartmentRequest addDepartmentRequest);
        (object data, string message) UpdateDepartment(string departmentcode, AddDepartmentRequest addDepartmentRequest);
        (object data, string message) GetUserDepartment();
        (object data, string message) RemoveDepartment(string departmentcode);
    }
    public class DepartmentService : IDepartmentService
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        public DepartmentService(IConfiguration _config, MariaDBContext _dBContext)
        {
            config = _config;
            dBContext = _dBContext;
        }

        public (object data, string message) CreateDepartment(AddDepartmentRequest addDepartmentRequest)
        {
            Department department = dBContext.Departments.FirstOrDefault(x => x.Name.Equals(addDepartmentRequest.name));
            if (department != null)
                return (null, "Tên bộ phận đã tồn tại.");
            dBContext.Departments.Add(new Department()
            {
                Name = addDepartmentRequest.name,
                Code = ConvertToUnsign(addDepartmentRequest.name)
            });
            dBContext.SaveChanges();
            return ("Thành công!!!", "Tạo bộ phận mới thành công!!!");
        }

        public (object data, string message) GetUserDepartment()
        {
            List<Department> groups = dBContext.Departments.Include(x => x.Users).OrderByDescending(x => x.CreatedAt).ToList();
            var result = groups.Select(x => new
            {
                departmentid = x.ID,
                departmentname = x.Name,
                departmentcode = x.Code,
                departmentcreatedAt = x.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                users = x.Users.Select(v => new
                {
                    userid = v.UserId,
                    username = v.UserName,
                    userfullname = v.FullName
                }).ToList()
            });
            return (result, "Danh sách người dùng trong bộ phận.");

        }



        public static string ConvertToUnsign(string text)
        {
            text = text.Replace(" ", "").Trim();
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }



        public (object data, string message) UpdateDepartment(string departmentcode, AddDepartmentRequest addDepartmentRequest)
        {
            Department department = dBContext.Departments.FirstOrDefault(x => x.Code.Equals(departmentcode));
            if (department == null)
                return (null, "Mã bộ phận không tồn tại.");
            Department department2 = dBContext.Departments.FirstOrDefault(x => x.Name.Equals(addDepartmentRequest.name) && department.ID != x.ID);
            if(department2 != null)
                return (null, "Tên bộ phận đã tồn tại.");
            department.Name = addDepartmentRequest.name;
            dBContext.SaveChanges();
            return ("Thành công!!!", "Cập nhật tên phòng ban thành công!!!");
        }

        public (object data, string message) RemoveDepartment(string departmentcode)
        {
            Department department = dBContext.Departments.Include(x=>x.Users).FirstOrDefault(x => x.Code.Equals(departmentcode));
            if (department == null)
                return (null, "Mã bộ phận không tồn tại.");
            if(department.Users.Count != 0)
                return (null, "Bộ phận còn người dùng. Không được xóa.");
            department.Enable = true;
            dBContext.SaveChanges();
            return ("Thành công!!!", "Xóa bộ phận thành công!!!");
        }
    }
}
