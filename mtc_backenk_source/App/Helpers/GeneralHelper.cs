using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Project.Modules.Medias.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class GeneralHelper
{
    public static HttpClient HttpClient = new HttpClient();

    public static async Task<(string responseData, int? responseStatusCode)> SendRequestAsync(this HttpRequestMessage httpRequestMessage, string endpointURL, IDictionary<string, object> headers)
    {
        if (headers != null)
        {
            foreach (KeyValuePair<string, object> header in headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value.ToString());
            }
        }
        try
        {
            using HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            int responseStatusCode = (int)httpResponseMessage.StatusCode;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return (await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false), responseStatusCode);
            }
            else
            {
                return ($"Request to {endpointURL} error! StatusCode = {httpResponseMessage.StatusCode}", responseStatusCode);
            }
        }
        catch (Exception ex)
        {
            return (ex.Message, (int)HttpStatusCode.InternalServerError);
        }
    }

    public static (string responseData, int? responseStatusCode) SendRequestWithStringContent(this HttpMethod method, string endpointURL, string encodingData = null, IDictionary<string, object> headers = null, string mediaType = "application/json")
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, endpointURL);
        if (!(encodingData is null))
        {
            httpRequestMessage.Content = new StringContent(encodingData, Encoding.UTF8, mediaType);
        }
        return httpRequestMessage.SendRequestAsync(endpointURL, headers).Result;
    }

    public static (string responseData, int? responseStatusCode) SendRequestWithFormDataContent(this HttpMethod method, string endpointURL, MultipartFormDataContent multipartFormData = null, IDictionary<string, object> headers = null)
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, endpointURL);
        if (!(multipartFormData is null))
        {
            httpRequestMessage.Content = multipartFormData;
        }
        return httpRequestMessage.SendRequestAsync(endpointURL, headers).Result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext">
    /// Getting the current url of the request depends on whether you use Https or not?
    /// </param>
    /// <param name="useHttps">
    /// Option to use Https (The parameter is taken from the UseHttps property in appsetting.json):
    ///     There used: true
    ///     Do not use: false
    /// </param>
    /// <returns></returns>
    public static string GetBaseUrl(this HttpContext httpContext, bool useHttps)
    {
        if (useHttps)
        {
            return httpContext.Request.Scheme + "s://" + httpContext.Request.Host.Value;
        }
        else
        {
            return httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value;
        }
    }
    public static string RandomString(this int count)
    {
        string b = "abcdefghijklmnopqrstuvwxyz1234567890ASDFGHJKLMNBVCXZQWETYUIOP";
        Random ran = new Random();
        string random = "";
        for (int i = 0; i < count; i++)
        {
            int a = ran.Next(26);
            random += b.ElementAt(a);
        }
        return random;
    }
    public static string HashPassword(this string password)
    {
        using SHA512 sha = SHA512.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = sha.ComputeHash(inputBytes);
        // Convert the byte array to hexadecimal string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
        }
        return sb.ToString().ToLower();
    }

    public static T MergeData<T>(this T newData, T originData, string[] ignoreAttributes = null)
    {
        foreach (PropertyInfo propertyInfo in originData.GetType().GetProperties())
        {
            if (propertyInfo.GetValue(newData, null) != null && !ignoreAttributes.Any(x => x.Equals(propertyInfo.Name)))
            {
                propertyInfo.SetValue(originData, newData.GetType().GetProperty(propertyInfo.Name).GetValue(newData, null));
            }
        }
        return originData;
    }

    public static async Task<string> UploadFile(this IFormFile requestFile, string folderPath)
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

    public static async Task<(string ulr, string typeFile)> UploadFileOld(this IFormFile requestFile, string folderPath)
    {
        string typeFile = requestFile.ContentType;
        string fileFullName = new Random().Next() + "_" + Regex.Replace(requestFile.FileName.Trim(), @"[^a-zA-Z0-9_.]", "");
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        Directory.CreateDirectory(Path.Combine(webRootPath, folderPath)); // Tự động tạo dường dẫn thư mục nếu chưa có
        using (var stream = new FileStream(Path.Combine(webRootPath, folderPath, fileFullName), FileMode.Create))
        {
            await requestFile.CopyToAsync(stream);
            stream.Close();
        }
        return (fileFullName, typeFile);
    }

    public static (bool, string) DeleteFile(this string FileToDelete)
    {
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        try
        {
            string fullPathToDelete = Path.Combine(webRootPath, FileToDelete);
            if (File.Exists(fullPathToDelete))
            {
                File.Delete(fullPathToDelete);
                return (true, "File deleted successfully!");
            }
            else
            {
                return (false, "File no longer exists on the server!");
            }
        }
        catch (IOException ioExp)
        {
            return (false, ioExp.Message);
        }
    }

    public static string MD5Hash(this string text)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text)); // Compute hash from the bytes of text
        byte[] result = md5.Hash; // Get hash result after compute it  
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            strBuilder.Append(result[i].ToString("x2")); // Change it into 2 hexadecimal digits
        }
        return strBuilder.ToString();
    }

    public static bool PingIPDevice(this string targetHost, string data = "PingForTest")
    {
        Ping pingSender = new Ping();
        PingOptions options = new PingOptions
        {
            DontFragment = true
        };
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        PingReply reply = pingSender.Send(targetHost, 120, buffer, options);
        if (reply.Status == IPStatus.Success)
            return true;
        return false;
    }

    public static bool ValidateIPv4(this string DeviceIP)
    {
        if (String.IsNullOrWhiteSpace(DeviceIP))
            return false;
        string[] splitValues = DeviceIP.Split('.');
        if (splitValues.Length != 4)
            return false;
        return splitValues.All(r => byte.TryParse(r, out byte tempForParsing));
    }

    public static string GetMimeType(this string fileName)
    {
        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
        return contentType ?? "application/octet-stream";
    }

    public static TimeSpan ParseStringToTimeSpan(this string strInput)
    {
        try
        {
            return TimeSpan.Parse(strInput);
        }
        catch
        {
            //InvalidDate
            throw new InvalidDataException("InvalidTimeSpan");
        }
    }

    public static DateTime ParseStringGMTToDatime(this string strInput, string format = "yyyy-MM-dd HH:mm:ss")
    {
        try
        {
            return DateTime.Parse(strInput + " GMT").ToUniversalTime();
        }
        catch
        {
            //InvalidDate
            throw new InvalidDataException("InvalidDate");
        }
    }

    public static string GetURLVideo(string url)
    {
        string[] mediaExtensions = {
                        //".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", //etc
                        ".WAV", ".MID", ".MIDI", ".WMA", ".OGG", ".RMA", //etc
                        ".AVI", ".MP4", ".DIVX", ".WMV", //etc
                    };

        string fileNameExtension = "";
        string extension = "";
        int indexFileName = url.LastIndexOf("/");
        if (indexFileName > 0)
        {
            fileNameExtension = url.Substring(indexFileName + 1); //get file name
            extension = Path.GetExtension(fileNameExtension); //get dinh dang
        }

        bool checkVideo = false;
        if (Array.IndexOf(mediaExtensions, Path.GetExtension(extension.ToUpper()).ToUpperInvariant()) != -1) //check file media
        {
            checkVideo = true;
        }
        if (checkVideo)
        {
            string fileName = Path.GetFileNameWithoutExtension(fileNameExtension);
            return Path.Combine("uploads/medias", fileName, fileName + ".m3u8");
        }
        else
        {
            return url;
        }
    }


    public static string UrlCombine(String path1, String path2)
    {
        if (path2 == null) return null;
        return Path.Combine(path1, path2).Replace("\\", "/");
    }
    public static string UrlCombine(String path1, String path2, String path3)
    {
        return Path.Combine(path1, path2, path3).Replace("\\", "/");
    }

    public static string UrlCombine(String path1, String path2, String path3, String path4)
    {
        return Path.Combine(path1, path2, path3, path4).Replace("\\", "/");
    }

    public static string RandomColor()
    {
        Random _random = new Random();
        return String.Format("#{0:X6}", _random.Next(0x1000000));
    }
    public static async Task<(string ulr, string typeFile)> UploadFileV2(this IFormFile requestFile, string folderPath)
    {
        string typeFile = requestFile.ContentType;
        string fileFullName = new Random().Next() + "_" + Regex.Replace(requestFile.FileName.Trim(), @"[^a-zA-Z0-9_.]", "");
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        Directory.CreateDirectory(Path.Combine(webRootPath, folderPath)); // Tự động tạo dường dẫn thư mục nếu chưa có
        using (var stream = new FileStream(Path.Combine(webRootPath, folderPath, fileFullName), FileMode.Create))
        {
            await requestFile.CopyToAsync(stream);
            stream.Close();
        }
        return (fileFullName, typeFile);
    }
    public static string GetFileName(string strUrl)
    {
        return Path.GetFileName(strUrl);
    }

    public static string GetFileNameV2(this string strUrl)
    {
        return Path.GetFileName(strUrl);
    }
    public static string GetDirectoryFromFile(string strUrl)
    {
        string absolutePath = GeneralHelper.GetAbsolutePath(strUrl);
        return Path.GetDirectoryName(absolutePath);
    }
    private static string GetAbsolutePath(string strUrl)
    {
        Uri uri = new Uri(strUrl);
        string localPath = uri.LocalPath;
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + localPath;
    }
    public static int toInt(this string json)
    {
        try
        {
            if (String.IsNullOrEmpty(json)) return 0;
            return int.Parse(json.ToString());
        }
        catch (Exception)
        {

            return 0;
        }

    }
    public static string GetBaseURL(this HttpContext httpContext, int isHttps = 0)
    {
        if (isHttps == 0)
            return httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value;
        return httpContext.Request.Scheme + "s://" + httpContext.Request.Host.Value;
    }


    public static string GetFileNameWithoutExtension(string strUrl)
    {
        return Path.GetFileNameWithoutExtension(strUrl);
    }


    public static void ClearFolder(string strFolderPath)
    {
        DirectoryInfo dir = new DirectoryInfo(strFolderPath);
        if (dir is null) return;
        foreach (FileInfo fi in dir.GetFiles())
        {
            try
            {
                fi.Delete();
            }
            catch (Exception) { } // Ignore all exceptions
        }

        foreach (DirectoryInfo di in dir.GetDirectories())
        {
            ClearFolder(di.FullName);
            try
            {
                di.Delete();
            }
            catch (Exception) { } // Ignore all exceptions
        }
        dir.Delete();
    }



    public static string GetMD5File(string strUrl)
    {
        string absolutePath = GeneralHelper.GetAbsolutePath(strUrl);
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(absolutePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }


    public static void ffmpegRun(string command)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    //FileName = (@"D:\ffmpeg\bin\ffmpeg.exe"),
                    FileName = "ffmpeg",
                    Arguments = command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }

            };
            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


    }


    public static string ParsingToken(this HttpContext context)
    {
        var header = context.Request.Headers.Select(x => new { x.Key, x.Value }).ToList();
        var prepareToken = header.Where(x => x.Key == "Authorization").Select(x => x.Value).FirstOrDefault();
        if (String.IsNullOrEmpty(prepareToken)) return "";
        return prepareToken.ToString().Replace("Bearer ", "");
    }

    public static Media CombineMediaUrl(this Media media, string url)
    {
        if (media is null)
        {
            return null;
        }

        if (!media.MediaUrl.Contains(url))
        {
            media.MediaUrl = media.MediaTypeCode == "link" ? media.MediaUrl : $"{url}/{GetURLVideo(media.MediaUrl)}";
        }

        return media; 
    }
    public static string GetDeviceId(this HttpContext context)
    {
        var header = context.Request.Headers.Select(x => new { x.Key, x.Value }).ToList();
        var prepareToken = header.Where(x => x.Key == "DeviceId").Select(x => x.Value).FirstOrDefault();
        if (String.IsNullOrEmpty(prepareToken)) return "";
        return prepareToken.ToString();
    }

    public static (bool flagCheckToken, JwtSecurityToken dataToken) ValidateToken(this string token)
    {
        JwtSecurityToken dataToken = new JwtSecurityToken();
        try
        {
            dataToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch (Exception)
        {
            return (false, null);
        }
        return (true, dataToken);
    }

}
