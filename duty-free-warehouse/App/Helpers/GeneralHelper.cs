using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public static class GeneralHelper
    {
        private static HttpClient _client = new HttpClient();

        public const int OK = (int)HttpStatusCode.OK;
        public const int CREATED = (int)HttpStatusCode.Created;
        public const int ACCEPTED = (int)HttpStatusCode.Accepted;
        public const int NO_CONTENT = (int)HttpStatusCode.NoContent;
        public const int BAD_REQUEST = (int)HttpStatusCode.BadRequest;
        public const int UNAUTHORIZED = (int)HttpStatusCode.Unauthorized;
        public const int FORBIDDEN = (int)HttpStatusCode.Forbidden;
        public const int INTERNAL_SERVER_ERROR = (int)HttpStatusCode.InternalServerError;


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
                using (var response = await _client.SendAsync(httpRequestMessage).ConfigureAwait(false))
                {
                    int responseStatusCode = (int)response.StatusCode;
                    if (response.Content != null)
                        return (await response.Content.ReadAsStringAsync().ConfigureAwait(false), responseStatusCode);
                    if (response.IsSuccessStatusCode)
                        return (null, responseStatusCode);
                    else
                        return ($"Request to {endpointURL} error! StatusCode = {response.StatusCode}", responseStatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                return (ex.Message, INTERNAL_SERVER_ERROR);
            }
        }
        public static (string responseData, int? responseStatusCode) SendRequestWithStringContent(this HttpMethod method, string endpointURL, string encodingData = null, IDictionary<string, object> headers = null, string mediaType = "application/json")
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, endpointURL);
            if (!(encodingData is null))
                httpRequestMessage.Content = new StringContent(encodingData, Encoding.UTF8, mediaType);
            return httpRequestMessage.SendRequestAsync(endpointURL, headers).Result;
        }
        public static (string responseData, int? responseStatusCode) SendRequestWithFormDataContent(this HttpMethod method, string endpointURL, MultipartFormDataContent multipartFormData = null, IDictionary<string, object> headers = null)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, endpointURL);
            if (!(multipartFormData is null))
                httpRequestMessage.Content = multipartFormData;
            return httpRequestMessage.SendRequestAsync(endpointURL, headers).Result;
        }

        public static string MD5Hash(this string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.ASCII.GetBytes(text)); // Compute hash from the bytes of text
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
            if (string.IsNullOrWhiteSpace(DeviceIP))
                return false;
            string[] splitValues = DeviceIP.Split('.');
            if (splitValues.Length != 4)
                return false;
            return splitValues.All(r => byte.TryParse(r, out byte tempForParsing));
        }

        public static async Task<string> UploadFile(this IFormFile requestFile, string folderPath)
        {
            string fileFullName = 7.RandomString() + "_" + Regex.Replace(requestFile.FileName.Trim(), @"[^a-zA-Z0-9-_.]", "");
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(Path.Combine(webRootPath, folderPath)); // Tự động tạo dường dẫn thư mục nếu chưa có
            using (var stream = new FileStream(Path.Combine(webRootPath, folderPath, fileFullName), FileMode.Create))
            {
                await requestFile.CopyToAsync(stream);
                stream.Close();
            }
            return fileFullName;
        }

        public static string GetMimeType(this string fileName)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
            return contentType ?? "application/octet-stream";
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

        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return token == null ||
                    token.Type == JTokenType.Array && !token.HasValues ||
                    token.Type == JTokenType.Object && !token.HasValues ||
                    token.Type == JTokenType.String && token.ToString() == string.Empty ||
                    token.Type == JTokenType.Null;
        }

        public static string RandomString(this int count)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var rnd = new RNGCryptoServiceProvider();
            var sb = new StringBuilder();
            var buf = new byte[count];
            rnd.GetBytes(buf);
            foreach (byte b in buf)
                sb.Append(valid[b % 52]);
            return sb.ToString();
        }
    }
}