using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Project.App.Helpers;
using System;
using System.Collections.Generic;
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
using static Project.Modules.Accounts.Entities.Account;

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
            if (httpResponseMessage.Content != null)
            {
                return (await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false), responseStatusCode);
            }
            else if (httpResponseMessage.Content is null && httpResponseMessage.IsSuccessStatusCode)
            {
                return (null, responseStatusCode);
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

    public static T MergeData<T>(this object newData, T originData)
    {
        foreach (PropertyInfo propertyInfo in newData.GetType().GetProperties())
        {
            if (propertyInfo.GetValue(newData, null) != null && originData.GetType().GetProperties().Any(p => p.Name.Equals(propertyInfo.Name)))
            {
                originData.GetType().GetProperty(propertyInfo.Name).SetValue(originData, propertyInfo.GetValue(newData, null));
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
    public static string UrlCombine(this string pathRoot, string path1)
    {
        if (pathRoot == null) return "";
        return Path.Combine(pathRoot, path1).Replace("\\", "/");
    }

    
    public static async Task<(string path, string fullName)> UploadFileProAsync(this IFormFile requestFile, string folderPath)
    {
        _ = requestFile.ContentType;
        var fileFullName = new Random().Next() + "_" + Regex.Replace(requestFile.FileName.Trim(), @"[^a-zA-Z0-9_.]", "");
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        Directory.CreateDirectory(Path.Combine(webRootPath, folderPath)); // Tự động tạo dường dẫn thư mục nếu chưa có
        using (var stream = new FileStream(Path.Combine(webRootPath, folderPath, fileFullName), FileMode.Create))
        {
            await requestFile.CopyToAsync(stream);
            stream.Close();
        }
        return (folderPath.UrlCombine(fileFullName), fileFullName);
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
    public static string RandomString(this int count)
    {
        string b = "abcdefghijklmnopqrstuvwxyz";
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
    public static string OrderValue(this string sortColumn, string SortDir)
    {
        return sortColumn + " " + SortDir;
    }
    public static ResponseTable ShowTable(List<object> list,RequestTable request)
    {
        ResponseTable response = new ResponseTable
        {
            Data = request.Page == 0 ? list : list.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList(),
            Info = new Info
            {
                Page = request.Page,
                Limit = request.Limit,
                TotalRecord = list.Count,
            }
        };
        return response;
    }

}
