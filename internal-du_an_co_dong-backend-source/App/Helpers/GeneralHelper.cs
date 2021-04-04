using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Requests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GoogleTranslateFreeApi;
using System.IO.Compression;

public static class GeneralHelper
{
    public static HttpClient _client = new HttpClient();
    public static Dictionary<int, string> TypeLogAccess = new Dictionary<int, string>
    {
        {1, "Login" }
    };

    public const int OK = (int)HttpStatusCode.OK;
    public const int CREATED = (int)HttpStatusCode.Created;
    public const int ACCEPTED = (int)HttpStatusCode.Accepted;
    public const int NO_CONTENT = (int)HttpStatusCode.NoContent;
    public const int BAD_REQUEST = (int)HttpStatusCode.BadRequest;
    public const int UNAUTHORIZED = (int)HttpStatusCode.Unauthorized;
    public const int FORBIDDEN = (int)HttpStatusCode.Forbidden;
    public const int INTERNAL_SERVER_ERROR = (int)HttpStatusCode.InternalServerError;

    #region ที่ประสบความสำเร็จ

    public static void CopyTo(Stream src, Stream dest)
    {
        byte[] bytes = new byte[4096];

        int cnt;

        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
        {
            dest.Write(bytes, 0, cnt);
        }
    }

    public static byte[] Zip(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);

        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                //msi.CopyTo(gs);
                CopyTo(msi, gs);
            }

            return mso.ToArray();
        }
    }

    public static string Unzip(byte[] bytes)
    {
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                //gs.CopyTo(mso);
                CopyTo(gs, mso);
            }

            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }

    #endregion
    public static Language SwitchKeyLanguageISO693(string input)
    {
        #region ISO 693-1
        //ISO 693-1 Google Code
        switch (input)
        {
            case "af":
                return Language.Afrikaans;
            case "sq":
                return Language.Albanian;
            case "am":
                return Language.Amharic;
            case "ar":
                return Language.Arabic;
            case "hy":
                return Language.Armenian;
            case "az":
                return Language.Azerbaijani;
            case "eu":
                return Language.Belarusian;
            case "bn":
                return Language.Bengali;
            case "bs":
                return Language.Bosnian;
            case "bg":
                return Language.Bulgarian;
            case "ca":
                return Language.Catalan;
            case "ceb":
                return Language.Cebuano;
            case "zh-CN":
                return Language.ChineseSimplified;
            case "zh":
                return Language.ChineseSimplified;
            case "zh-TW":
                return Language.ChineseTraditional;
            case "co":
                return Language.Corsican;
            case "hr":
                return Language.Croatian;
            case "cs":
                return Language.Czech;
            case "da":
                return Language.Danish;
            case "nl":
                return Language.Dutch;
            case "en":
                return Language.English;
            case "eo":
                return Language.Esperanto;
            case "et":
                return Language.Estonian;
            case "fi":
                return Language.Finnish;
            case "fr":
                return Language.French;
            case "fy":
                return Language.Frisian;
            case "gl":
                return Language.Galician;
            case "ka":
                return Language.Georgian;
            case "de":
                return Language.German;
            case "el":
                return Language.Greek;
            case "gu":
                return Language.Gujarati;
            case "ht":
                return Language.HaitianCreole;
            case "ha":
                return Language.Hausa;
            case "haw":
                return Language.Hawaiian;
            case "he":
                return Language.Hebrew;
            case "iw":
                return Language.Hebrew;
            case "hi":
                return Language.Hindi;
            case "hmn":
                return Language.Hmong;
            case "hu":
                return Language.Hungarian;
            case "is":
                return Language.Icelandic;
            case "ig":
                return Language.Igbo;
            case "id":
                return Language.Indonesian;
            case "ga":
                return Language.Irish;
            case "it":
                return Language.Italian;
            case "ja":
                return Language.Japanese;
            case "jv":
                return Language.Javanese;
            case "kn":
                return Language.Kannada;
            case "kk":
                return Language.Kazakh;
            case "km":
                return Language.Khmer;
            //case "rw":
            //    return Language.Kinyarwanda;
            case "ko":
                return Language.Korean;
            case "ku":
                return Language.KurdishKurmanji;
            case "ky":
                return Language.Kyrgyz;
            case "lo":
                return Language.Lao;
            case "la":
                return Language.Latin;
            case "lv":
                return Language.Latvian;
            case "lt":
                return Language.Lithuanian;
            case "lb":
                return Language.Luxembourgish;
            case "mk":
                return Language.Macedonian;
            case "mg":
                return Language.Malagasy;
            case "ms":
                return Language.Malay;
            case "ml":
                return Language.Malayalam;
            case "mt":
                return Language.Maltese;
            case "mi":
                return Language.Maori;
            case "mr":
                return Language.Marathi;
            case "mn":
                return Language.Mongolian;
            case "my":
                return Language.MyanmarBurmese;
            case "ne":
                return Language.Nepali;
            case "no":
                return Language.Norwegian;
            case "ny":
                return Language.Chichewa;
            //case "or":
            //    return Language.Odia(Oriya);
            case "ps":
                return Language.Pashto;
            case "fa":
                return Language.Persian;
            case "pl":
                return Language.Polish;
            case "pt":
                return Language.Portuguese;
            case "pa":
                return Language.Punjabi;
            case "ro":
                return Language.Romanian;
            case "ru":
                return Language.Russian;
            case "sm":
                return Language.Samoan;
            case "gd":
                return Language.ScotsGaelic;
            case "sr":
                return Language.Serbian;
            case "st":
                return Language.Sesotho;
            case "sn":
                return Language.Shona;
            case "sd":
                return Language.Sindhi;
            case "si":
                return Language.Sinhala;
            case "sk":
                return Language.Slovak;
            case "sl":
                return Language.Slovenian;
            case "so":
                return Language.Somali;
            case "es":
                return Language.Spanish;
            case "su":
                return Language.Sundanese;
            case "sw":
                return Language.Swahili;
            case "sv":
                return Language.Swedish;
            case "tl":
                return Language.Filipino;
            case "tg":
                return Language.Tajik;
            case "ta":
                return Language.Tamil;
            //case "tt":
            //    return Language.Tatar;
            case "te":
                return Language.Telugu;
            case "th":
                return Language.Thai;
            case "tr":
                return Language.Turkish;
            //case "tk":
            //    return Language.Turkmen;
            case "uk":
                return Language.Ukrainian;
            case "ur":
                return Language.Urdu;
            //case "ug":
            //    return Language.Uyghur;
            case "uz":
                return Language.Uzbek;
            case "vi":
                return Language.Vietnamese;
            case "cy":
                return Language.Welsh;
            case "xh":
                return Language.Xhosa;
            case "yi":
                return Language.Yiddish;
            case "yo":
                return Language.Yoruba;
            case "zu":
                return Language.Zulu;
            default:
                return Language.Vietnamese;
        }
        #endregion
    }
    public static async Task<string> TranslateText(string input, Language fromlanguage, Language tolanguage)
    {
        GoogleTranslator googleTranslator = new GoogleTranslator();
        TranslationResult translationResult = await googleTranslator.TranslateAsync(input, fromlanguage, tolanguage);
        return translationResult.MergedTranslation;
    }
    public static T CheckUpdateObject<T>(T originalObj, T updateObj)
    {
        foreach (var property in updateObj.GetType().GetProperties())
        {
            var xxx = property.GetValue(updateObj, null);

            if (property.GetValue(updateObj, null) == null || (property.Name.Equals("CreateAt")) || (property.Name.Equals("CreatedAt")) || (property.GetValue(updateObj, null) != null && property.GetValue(updateObj, null).GetType() == typeof(int) && int.Parse(property.GetValue(updateObj, null).ToString()) == 0))
            {
                property.SetValue(updateObj, originalObj.GetType().GetProperty(property.Name)
                .GetValue(originalObj, null));
            }
        }
        return updateObj;
    }
    public static string generateID()
    {
        return Guid.NewGuid().ToString();
        //return Guid.NewGuid().ToString("N");
    }
    public static async Task<(string responseData, int? responseStatusCode)> SendRequestAsync2(this HttpMethod method, string url, object data = null, IDictionary<string, object> headers = null)
    {
        var requestMessage = new HttpRequestMessage(method, url);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value.ToString());
            }
        }
        if (data != null)
        {
            if (data.GetType() == typeof(string))
                requestMessage.Content = new StringContent((string)data, Encoding.UTF8, "application/json");
            else
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
        try
        {
            using (var response = await _client.SendAsync(requestMessage).ConfigureAwait(false))
            {
                int responseStatusCode = (int)response.StatusCode;
                if (response.Content != null)
                    return (await response.Content.ReadAsStringAsync().ConfigureAwait(false), responseStatusCode);
                if (response.IsSuccessStatusCode)
                {
                    return (null, responseStatusCode);
                }
                else
                {
                    return ($"Request to {url} error ! StatusCode = {response.StatusCode}", responseStatusCode);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            return (ex.Message, INTERNAL_SERVER_ERROR);
        }
    }
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
            using var response = await _client.SendAsync(httpRequestMessage).ConfigureAwait(false);
            int responseStatusCode = (int)response.StatusCode;
            if (response.Content != null)
                return (await response.Content.ReadAsStringAsync().ConfigureAwait(false), responseStatusCode);
            if (response.IsSuccessStatusCode)
                return (null, responseStatusCode);
            else
                return ($"Request to {endpointURL} error! StatusCode = {response.StatusCode}", responseStatusCode);
        }
        catch (HttpRequestException ex)
        {
            return (ex.Message, INTERNAL_SERVER_ERROR);
        }
    }
    public static async Task<(string responseData, int? responseStatusCode)> SendRequestWithStringContentAsync(this HttpMethod method, string endpointURL, string encodingData = null, IDictionary<string, object> headers = null, string mediaType = "application/json")
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, endpointURL);
        if (!(encodingData is null))
            httpRequestMessage.Content = new StringContent(encodingData, Encoding.UTF8, mediaType);
        (string data, int? statusCode) = await httpRequestMessage.SendRequestAsync(endpointURL, headers);

        try
        {
            JObject jData = JObject.Parse(data);
            if (jData["message"] != null && statusCode != 200)
            {
                return (jData["message"].ToString(), statusCode);
            }
            return (data, statusCode);
        }
        catch
        {
            return (data, 400);
        }
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
    public static async Task<string> UploadFile(this IFormFile requestFile, string folderPath)
    {
        folderPath = folderPath.Substring(1);
        string fileFullName = new Random().Next() +DateTime.Now.ToString().Replace(" ","").Replace("/","").Replace(":","")+"_" + Regex.Replace(requestFile.FileName.Trim(), @"[^a-zA-Z0-9-_.]", "");
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        string pathCreateFolder = Path.Combine(webRootPath, folderPath);
        Directory.CreateDirectory(pathCreateFolder); // Tự động tạo dường dẫn thư mục nếu chưa có
        string pathCreateFile = Path.Combine(webRootPath, folderPath, fileFullName);
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
            FileToDelete = FileToDelete.Substring(1);
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
    public static string CopyFile(this byte[] file, string folderPath, string type)
    {
        string fileFullName = ((long)TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds).ToString() + type;
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        Directory.CreateDirectory(Path.Combine(webRootPath, folderPath)); // Tự động tạo dường dẫn thư mục nếu chưa có
        File.WriteAllBytes(Path.Combine(webRootPath, folderPath, fileFullName), file);
        return $"{folderPath}/{fileFullName}";
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
    public static string RandomOTPKnorr(this int count)
    {
        string b = "1234567890";
        Random ran = new Random();
        string random = "";
        for (int i = 0; i < count; i++)
        {
            int a = ran.Next(10);
            random += b.ElementAt(a);
        }
        return random;
    }
    public static DateTime? FormatStringDateTime(this string dateTimeInput)
    {
        if (String.IsNullOrEmpty(dateTimeInput))
        {
            return null;
        }
        DateTime.TryParseExact(dateTimeInput, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime result);
        return result;
    }
    public static long ConvertTimeSpanToLong(this TimeSpan timeSpan)
    {
        var time = timeSpan.TotalMilliseconds;
        return Convert.ToInt64(time);
    }
    public static string ConvertDateTimeToString(this DateTime? DateTimeInput, string format = "dd/MM/yyyy HH:mm:ss")
    {
        if (DateTimeInput == null) return "";
        return DateTimeInput.Value.ToString(format);
    }
    public static long ConvertDateTimeToSecond(this DateTime dateTime)
    {
        //var seconds  = dateTime.ToUnixTimeSeconds();

        //double result = dateTime.Subtract(DateTime.MinValue).TotalSeconds;
        TimeSpan span = dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local));
        return Convert.ToInt64(span.TotalSeconds);

    }
    public static int toInt(this JObject json)
    {
        return int.Parse(json.ToString());
    }
    public static int toInt(this JToken json)
    {
        return int.Parse(json.ToString());
    }
    public static int toInt(this string json)
    {
        if (String.IsNullOrEmpty(json)) return 0;
        return int.Parse(json.ToString());
    }
    public static int toHttps(this string json)
    {
        if (String.IsNullOrEmpty(json)) return 0;
        return int.Parse(json.ToString());
    }
    public static int convertStock(this int? value)
    {
        if (!value.HasValue) return 0;
        return value.Value;
    }
    public static int convertStock(this List<int?> value)
    {
        if (value.Count == 0) return 0;
        int result = 0;
        foreach (var item in value)
        {
            if (!item.HasValue) continue;
            result = result + item.Value;
        }
        return result;
    }
    public static string toDefaultUrl(this string json, string urlDefault = "")
    {
        if (String.IsNullOrEmpty(json)) return urlDefault;
        return json.ToString();
    }
    public static int toEventId(this string json)
    {
        if (String.IsNullOrEmpty(json)) return 6;
        return int.Parse(json.ToString());
    }
    public static long TimeExpiredOTP(this string json)
    {
        if (String.IsNullOrEmpty(json)) return 300;
        return long.Parse(json.ToString());
    }
    public static long toLong(this JToken json)
    {
        return long.Parse(json.ToString());
    }
    public static long toLong(this JObject json)
    {
        return long.Parse(json.ToString());
    }
    public static string GetFileName(string strUrl)
    {
        return Path.GetFileName(strUrl);
    }
    public static async Task<(string ulr, string typeFile)> UploadFileDevHung(this IFormFile requestFile, string folderPath)
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
    public static string UrlCombine(this String path1, String path2)
    {
        if (String.IsNullOrEmpty(path2) || String.IsNullOrEmpty(path1)) return "";
        return Path.Combine(path2, path1).Replace("\\", "/");
    }
    public static string UrlCombine(String path1, String path2, String path3)
    {
        return Path.Combine(path1, path2, path3).Replace("\\", "/");
    }
    public static string UrlCombine(String path1, String path2, String path3, String path4)
    {
        return Path.Combine(path1, path2, path3, path4).Replace("\\", "/");
    }
    public static string GetBaseURL(this HttpContext httpContext, int isHttps = 0)
    {
        if (isHttps == 0)
            return httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value;
        return httpContext.Request.Scheme + "s://" + httpContext.Request.Host.Value;
        //HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
    }
    public static bool CheckEmail(this string emailInput)
    {
        Regex urlRx = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9-]+.+.[A-Z]{2,4}", RegexOptions.IgnoreCase);
        Match match = urlRx.Match(emailInput);
        if (!match.Success)
        {
            return false;
        }
        return true;
    }
    public static string GenerateId(List<string> type = null, int length = 6)
    {
        if (type is null) type = new List<string>() { "numbers" };
        JObject inputCode = new JObject
            {
                new JProperty("alphabets", "ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                new JProperty("small_alphabets", "abcdefghijklmnopqrstuvwxyz"),
                new JProperty("numbers", "1234567890"),
            };

        string characters = "";
        foreach (var x in type)
        {
            characters += inputCode[x];
        }

        string otp = string.Empty;
        for (int i = 0; i < length; i++)
        {
            string character = string.Empty;
            do
            {
                int index = new Random().Next(0, characters.Length);
                character = characters.ToCharArray()[index].ToString();
            } while (otp.IndexOf(character) != -1);
            otp += character;
        }
        return otp;
    }
    public static string UploadFolder(this string folderPath, string folderName)
    {
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        if (String.IsNullOrEmpty(folderPath))
        {
            string path2 = Path.Combine(webRootPath, folderName);
            Directory.CreateDirectory(path2); // Tự động tạo dường dẫn thư mục nếu chưa có

        }
        else
        {
            string path = $"{webRootPath}/{folderPath}/{folderName}";
            Directory.CreateDirectory(path); // Tự động tạo dường dẫn thư mục nếu chưa có
        }
        //if (Directory.Exists(Path.Combine(webRootPath, folderPath)))
        //{
        //    return null;
        //}
        return folderPath;
    }
    public static string UpdateFolder(this string folderPath, string folderName, string folderNameNew)
    {

        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        if (String.IsNullOrEmpty(folderPath))
        {

            string path2Old = Path.Combine(webRootPath, folderName);
            string path2New = Path.Combine(webRootPath, folderNameNew);
            if (Directory.Exists(path2Old))
            {
                return null;
            }
            Directory.Move(path2Old, path2New); // Tự động tạo dường dẫn thư mục nếu chưa có

        }
        else
        {
            string pathOld = $"{webRootPath}/{folderPath}/{folderName}";
            string pathNew = $"{webRootPath}/{folderPath}/{folderNameNew}";
            if (Directory.Exists(pathOld))
            {
                return null;
            }
            Directory.Move(pathOld, pathNew); // Tự động tạo dường dẫn thư mục nếu chưa có
        }

        return folderPath;
    }
    public static bool DeletePath(this string FileToDelete, string folderName)
    {
        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        string path = $"{webRootPath}{FileToDelete}/{folderName}";
        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path);
            }
            catch
            {

                Directory.Delete(path, true);
                return true;
            }
            return true;
        }
        return false;
    }
    public static string GetLocalPathUrl(this string strUrl)
    {
        if (string.IsNullOrEmpty(strUrl)) return "";
        try
        {
            Uri uri = new Uri(strUrl);
            return uri.LocalPath.Remove(0, 1);
        }
        catch (Exception)
        {

            return "";
        }

    }
    public static string GetExtensionFile(this string pathFile)
    {
        return Path.GetExtension(pathFile);
    }







}
