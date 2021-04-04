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
    public interface IModuleService
    {
        (object data, string message) CreateModule(AddModuleRequest moduleRequest);
        (object data, string message) GetPermissionModule();
    }

    public class ModuleService : IModuleService
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        public ModuleService(IConfiguration _config, MariaDBContext _dBContext)
        {
            config = _config;
            dBContext = _dBContext;
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


        public (object data, string message) CreateModule(AddModuleRequest moduleRequest)
        {
            Module module = dBContext.Modules.FirstOrDefault(x => x.Name.Equals(moduleRequest.name));
            if (module != null)
                return (null, "Tên module đã tồn tại");
            dBContext.Modules.Add(new Module()
            {
                Name = moduleRequest.name,
                Code = ConvertToUnsign(moduleRequest.name)
            });
            dBContext.SaveChanges();
            return ("Thành công!!!", "Tạo mới moudle thành công!!!");
        }

        public (object data, string message) GetPermissionModule()
        {
            List<Module> permissions = dBContext.Modules.Include(x => x.Permissions).OrderByDescending(x => x.CreatedAt).ToList();
            var result = permissions.Select(x => new
            {
                moduleid = x.ID,
                modulename = x.Name,
                modulecode = x.Code,
                moduleCreatedAt = x.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                permissions = x.Permissions.Select(v => new {
                    id = v.ID,
                    code = v.Code,
                    createdAt = v.CreatedAt,
                    name = v.Name
                }).ToList()
            });
            return (result, "Thông tin danh sách quyền của bộ phận.");
        }
    }
}
