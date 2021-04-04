using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Helpers;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Services
{
    public interface IMediaService
    {
        List<Media> UploadFile(List<MediaServiceReponse> mediaServiceReponses);
        List<MediaResponse> ShowAll();
        (MediaResponse data, string message) StoreMedia(IFormFile MediaFiles);
        (MediaResponse data, string message) StoreMediaWithByte(byte[] MediaFile, string fileSource);
        (object data, string message) Delete(string path);
        Task<(bool check, string fullPath, object data)> UploadFileAWS(string bucket, IFormFile file, string path);
        Task<(byte[] data, string message, object data1)> GetFile(string bucket, string fileFullName);
        string GetUrl(string bucket, string path);
    }
    public class MediaService : IMediaService
    {
        readonly private IConfiguration configuration;
        private readonly IRepositoryWrapperMariaDB repositoryWrapperMariaDB;
        private readonly string accessKeyId;
        private readonly string secretAccessKey;
        private readonly IAmazonS3 client;

        public MediaService(IConfiguration _configuration, IRepositoryWrapperMariaDB _repositoryWrapperMariaDB)
        {
            configuration = _configuration;
            repositoryWrapperMariaDB = _repositoryWrapperMariaDB;
            accessKeyId = configuration["OutsideSystems:AWS_S3:ACCESS_KEY_ID"];
            secretAccessKey = configuration["OutsideSystems:AWS_S3:SECRET_ACCESS_KEY"];
            var s3Config = new AmazonS3Config
            {
                ServiceURL = configuration["OutsideSystems:AWS_S3:SERVICE_URL"],
                ForcePathStyle = true
            };
            client = new AmazonS3Client(accessKeyId, secretAccessKey, s3Config);
        }
        public async Task<(bool check, string fullPath, object data)> UploadFileAWS(string bucket, IFormFile file, string path)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(client);

                using var newMemoryStream = new MemoryStream();
                file.CopyTo(newMemoryStream);
                string fileName = DateTime.UtcNow.Ticks.ToString() + "_" + Regex.Replace(file.FileName.Trim(), @"[^a-zA-Z0-9-_.]", "");
                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = newMemoryStream,
                    BucketName = bucket,
                    Key = path + "/" + fileName
                };

                PutObjectResponse response = await client.PutObjectAsync(request);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return (false, null, null);
                }
                return (true, request.Key, response);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e.Message);
                return (false, e.Message, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (false, null, null);
            }

        }

        public (object data, string message) Delete(string path)
        {
            string urlUploadMedia = configuration["OutsideSystems:Media:Url"] + "api/media/deleteByPath";
            string obj = JsonConvert.SerializeObject(new
            {
                mediaUrl = path
            });

            (string data, int? statusCode) = HttpMethod.Post.SendRequestWithStringContent(urlUploadMedia, obj);
            JObject jObject;
            try
            {
                jObject = JObject.Parse(data);
            }
            catch (Exception)
            {

                return (null, data);
            }
            if (statusCode != (int)HttpStatusCode.OK)
            {
                return (null, jObject["message"].ToString());
            }
            return (jObject["data"], jObject["message"].ToString());
        }

        public List<MediaResponse> ShowAll()
        {
            IQueryable<Media> medias = repositoryWrapperMariaDB.Medias.FindAll();
            List<MediaResponse> mediaResponses = medias.Select(x => new MediaResponse(x)).ToList();
            return mediaResponses;
        }

        public (MediaResponse data, string message) StoreMedia(IFormFile MediaFile)
        {
            MultipartFormDataContent multipartFormDataContent;
            string folderSave = "/upload/" + DateTime.Now.ToString("yyyyMMdd");
            string appId = configuration["OutsideSystems:Media:AppId"];
            string typeStored = configuration["OutsideSystems:Media:TypeStored"];
            string urlUploadMedia = configuration["OutsideSystems:Media:Url"] + "/api/media/upload";
            float maximumSizeUpload = float.Parse(configuration["OutsideSystems:Media:MaximumSize"]);
            List<MediaServiceReponse> mediaServiceReponses = new List<MediaServiceReponse>();
            byte[] fileBytes = new Byte[MediaFile.Length];
            MediaFile.OpenReadStream().Read(fileBytes, 0, Int32.Parse(MediaFile.Length.ToString()));

            if (!MediaFile.ContentType.Contains("image"))
            {
                return (null, "InvalidContentType");
            }

            if (!CheckContentTypeImage(fileBytes, MediaFile.FileName))
            {
                return (null, "InvalidContentType");
            }

            if (((float)MediaFile.Length / 1024 / 1024) > maximumSizeUpload)
            {
                return (null, "InvalidMaxSizeUpload");
            }
            multipartFormDataContent = new MultipartFormDataContent
                {
                    { new StreamContent(MediaFile.OpenReadStream()), "mediaFiles", MediaFile.FileName },
                    { new StringContent(folderSave), "folderSave" },
                    { new StringContent(appId), "applicationId" },
                    { new StringContent(typeStored), "typeStored" },
                };
            (string data, int? statuCode) = HttpMethod.Post.SendRequestWithFormDataContent(urlUploadMedia, multipartFormDataContent);
            if (statuCode != (int)HttpStatusCode.OK)
            {
                JObject jData = JObject.Parse(data);
                return (null, jData["message"].ToString());
            }
            JObject jObject = JObject.Parse(data);
            string obj = JsonConvert.SerializeObject(jObject["data"][0]);
            mediaServiceReponses.Add(JsonConvert.DeserializeObject<MediaServiceReponse>(obj));
            List<Media> medias = UploadFile(mediaServiceReponses);
            MediaResponse mediaResponse = new MediaResponse(medias.FirstOrDefault());
            return (mediaResponse, "StoreMediaSuccess");
        }

        public (MediaResponse data, string message) StoreMediaWithByte(byte[] MediaFile, string fileSource)
        {
            MultipartFormDataContent multipartFormDataContent;
            string folderSave = "/upload/" + DateTime.Now.ToString("yyyyMMdd");
            string appId = configuration["OutsideSystems:Media:AppId"];
            string typeStored = configuration["OutsideSystems:Media:TypeStored"];
            string urlUploadMedia = configuration["OutsideSystems:Media:Url"] + "/api/media/upload";
            List<MediaServiceReponse> mediaServiceReponses = new List<MediaServiceReponse>();
            multipartFormDataContent = new MultipartFormDataContent
                {
                    { new StringContent(folderSave), "folderSave" },
                    { new StringContent(appId), "applicationId" },
                    { new StringContent(typeStored), "typeStored" },
                };
            multipartFormDataContent.Add(new ByteArrayContent(MediaFile), "mediaFiles", fileSource);
            (string data, int? statuCode) = HttpMethod.Post.SendRequestWithFormDataContent(urlUploadMedia, multipartFormDataContent);
            if (statuCode != (int)HttpStatusCode.OK)
            {
                JObject jData = JObject.Parse(data);
                return (null, jData["message"].ToString());
            }
            JObject jObject = JObject.Parse(data);
            string obj = JsonConvert.SerializeObject(jObject["data"][0]);
            mediaServiceReponses.Add(JsonConvert.DeserializeObject<MediaServiceReponse>(obj));
            List<Media> medias = UploadFile(mediaServiceReponses);
            MediaResponse mediaResponse = new MediaResponse(medias.FirstOrDefault());
            return (mediaResponse, "StoreMediaSuccess");
        }

        public List<Media> UploadFile(List<MediaServiceReponse> mediaServiceReponses)
        {
            List<Media> medias = new List<Media>();
            foreach (var item in mediaServiceReponses)
            {
                medias.Add(new Media
                {
                    MediaName = item.ObjectName,
                    MediaPath = item.ObjectPath,
                    MediaTitle = System.IO.Path.ChangeExtension(item.ObjectName, null)
                });
            }

            repositoryWrapperMariaDB.Medias.AddRange(medias);
            repositoryWrapperMariaDB.SaveChanges();
            return medias;
        }

        private bool CheckContentTypeImage(byte[] file, string fileName)
        {
            byte[] BMP = { 66, 77 };
            //byte[] DOC = { 208, 207, 17, 224, 161, 177, 26, 225 };
            //byte[] EXE_DLL = { 77, 90 };
            byte[] GIF = { 71, 73, 70, 56 };
            byte[] ICO = { 0, 0, 1, 0 };
            byte[] JPG = { 255, 216, 255 };
            //byte[] MP3 = { 255, 251, 48 };
            //byte[] OGG = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
            //byte[] PDF = { 37, 80, 68, 70, 45, 49, 46 };
            byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
            //byte[] RAR = { 82, 97, 114, 33, 26, 7, 0 };
            //byte[] SWF = { 70, 87, 83 };
            byte[] TIFF = { 73, 73, 42, 0 };
            byte[] WEBP = { 82, 73, 70, 70 };
            //byte[] TORRENT = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
            //byte[] TTF = { 0, 1, 0, 0, 0 };
            //byte[] WAV_AVI = { 82, 73, 70, 70 };
            //byte[] WMV_WMA = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };
            //byte[] ZIP_DOCX = { 80, 75, 3, 4 };

            string mime = "application/octet-stream"; //DEFAULT UNKNOWN MIME TYPE

            //Ensure that the filename isn't empty or null
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            //Get the file extension
            //string extension = Path.GetExtension(fileName) == null ? string.Empty : Path.GetExtension(fileName).ToUpper();

            //Get the MIME Type
            if (file.Take(2).SequenceEqual(BMP))
            {
                mime = "image/bmp";
            }
            //else if (file.Take(8).SequenceEqual(DOC))
            //{
            //    mime = "application/msword";
            //}
            //else if (file.Take(2).SequenceEqual(EXE_DLL))
            //{
            //    mime = "application/x-msdownload"; //both use same mime type
            //}
            else if (file.Take(4).SequenceEqual(GIF))
            {
                mime = "image/gif";
            }
            else if (file.Take(4).SequenceEqual(ICO))
            {
                mime = "image/x-icon";
            }
            else if (file.Take(3).SequenceEqual(JPG))
            {
                mime = "image/jpeg";
            }
            //else if (file.Take(3).SequenceEqual(MP3))
            //{
            //    mime = "audio/mpeg";
            //}
            //else if (file.Take(14).SequenceEqual(OGG))
            //{
            //    if (extension == ".OGX")
            //    {
            //        mime = "application/ogg";
            //    }
            //    else if (extension == ".OGA")
            //    {
            //        mime = "audio/ogg";
            //    }
            //    else
            //    {
            //        mime = "video/ogg";
            //    }
            //}
            //else if (file.Take(7).SequenceEqual(PDF))
            //{
            //    mime = "application/pdf";
            //}
            else if (file.Take(16).SequenceEqual(PNG))
            {
                mime = "image/png";
            }
            //else if (file.Take(7).SequenceEqual(RAR))
            //{
            //    mime = "application/x-rar-compressed";
            //}
            //else if (file.Take(3).SequenceEqual(SWF))
            //{
            //    mime = "application/x-shockwave-flash";
            //}
            else if (file.Take(4).SequenceEqual(TIFF))
            {
                mime = "image/tiff";
            }
            else if (file.Take(4).SequenceEqual(WEBP))
            {
                mime = "image/webp";
            }
            //else if (file.Take(11).SequenceEqual(TORRENT))
            //{
            //    mime = "application/x-bittorrent";
            //}
            //else if (file.Take(5).SequenceEqual(TTF))
            //{
            //    mime = "application/x-font-ttf";
            //}
            //else if (file.Take(4).SequenceEqual(WAV_AVI))
            //{
            //    mime = extension == ".AVI" ? "video/x-msvideo" : "audio/x-wav";
            //}
            //else if (file.Take(16).SequenceEqual(WMV_WMA))
            //{
            //    mime = extension == ".WMA" ? "audio/x-ms-wma" : "video/x-ms-wmv";
            //}
            //else if (file.Take(4).SequenceEqual(ZIP_DOCX))
            //{
            //    mime = extension == ".DOCX" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" : "application/x-zip-compressed";
            //}

            return mime.Contains("image");
        }

        //30.12
        public async Task<(byte[] data, string message, object data1)> GetFile(string bucket, string fileFullName)
        {
            try
            {
                GetObjectRequest getObjectResponse = new GetObjectRequest()
                {
                    BucketName = bucket,
                    Key = fileFullName
                };
                GetObjectResponse response = await client.GetObjectAsync(getObjectResponse);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return (null, "An error occurred", null);
                }

                return (ReadToEnd(response.ResponseStream), "Success", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetFile-S3:{ex.Message} + {ex.InnerException} + {ex.StackTrace}");
                return (null, "File not found", null);
            }
        }

        public string GetUrl(string bucket, string path)
        {
            try
            {

                string urlBase = client.GeneratePreSignedURL(bucket, path, DateTime.UtcNow.AddDays(1), new Dictionary<string, object>());
                string privateUrl = configuration["OutsideSystems:AWS_S3:SERVICE_URL"].Replace("http", "https");
                var result = urlBase.Replace(privateUrl,configuration["OutsideSystems:AWS_S3:PUBLIC_URL"]);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUrl:s3:{ex.Message}{ex.InnerException}");
                return "";
            }
        }

        private byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }


}
