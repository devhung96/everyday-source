using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public static class FilesExtensions
    {
        public static async Task<string> UploadFileLocal(this IFormFile requestFile, string folderPath)
        {
            string fileFullName = new Random().Next() + "_" + Regex.Replace(requestFile.FileName.Trim(), @"[^a-zA-Z0-9-_.]", "");
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(Path.Combine(webRootPath, folderPath)); // Tự động tạo dường dẫn thư mục nếu chưa có
            using (var stream = new FileStream(Path.Combine(webRootPath, folderPath, fileFullName), FileMode.Create))
            {
                await requestFile.CopyToAsync(stream);
                stream.Close();
            }
            return fileFullName;
        }

        public static bool AllowExtension(this string extension)
        {
            var _extensionAllow = new string[] { ".jpeg", ".jpg", ".png" };
            return _extensionAllow.Any(x => x.Equals(extension));
        }


        public static async Task<( string fullPath, string fileName, string message)> UploadFileLocalAndUnzipAsync(IFormFile fileZip)
        {
            ///
            try
            {
                //Update file unzip 
                byte[] fileZipBytes = new Byte[fileZip.Length];
                fileZip.OpenReadStream().Read(fileZipBytes, 0, Int32.Parse(fileZip.Length.ToString()));
                string fileName = await fileZip.UploadFileLocal("upload");

                string zipSoure = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", fileName);
                string zipDestination = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","upload", Path.GetFileNameWithoutExtension(zipSoure));

                ZipFile.ExtractToDirectory(zipSoure, zipDestination);
                File.Delete(zipSoure);

                return (zipDestination, fileName, "Upload success");
            }
            catch (Exception ex)
            {
                return ("","", $"Upload faild:{ex.Message}:{ex.StackTrace}");
            }
        }

    }
}
