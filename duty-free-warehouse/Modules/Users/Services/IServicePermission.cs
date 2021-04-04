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
    public interface IServicePermission
    {
        (object data, string message) CreatePermission(AddPermissionRequest permissionRequest);

    }

    public class ServicePermission : IServicePermission
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        public ServicePermission(IConfiguration _config, MariaDBContext _dBContext)
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

        public (object data, string message) CreatePermission(AddPermissionRequest permissionRequest)
        {
            Permission permission = dBContext.Permissions.Include(x => x.Module).FirstOrDefault(x => x.Name.Equals(permissionRequest.name) && x.Module.Code == permissionRequest.modulecode);
            if (permission != null)
                return (null, "Tên quyền đã tồn tại trong module");
            dBContext.Permissions.Add(new Permission()
            {
                Name = permissionRequest.name,
                ModuleID = dBContext.Modules.FirstOrDefault(x => x.Code == permissionRequest.modulecode).ID,
                Code = ConvertToUnsign(permissionRequest.name)
            });
            dBContext.SaveChanges();
            return ("Thành công!!!", "Tạo mới quyền trong module thành công!!!");
        }

    }
}
