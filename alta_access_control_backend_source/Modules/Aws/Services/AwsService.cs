using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project.Modules.Aws.Services
{
    public interface IAwsService
    {
        Task<(bool check, string fullPath, object data)> Upload(string bucket, IFormFile file, string path);
        Task<(byte[] data, string message, object data1)> GetFile(string fileFullName);
        Task<(bool check, string message)> DeleteFile(string fileFullName);
    }
    public class AwsService : IAwsService
    {
        private readonly IConfiguration Configuration;
        private readonly string accessKeyId;
        private readonly string secretAccessKey;
        private readonly string Bucket;
        private readonly IAmazonS3 client;
        public AwsService(IConfiguration configuration)
        {
            Configuration = configuration;
            accessKeyId = Configuration["OutsideSystems:AWS_S3:ACCESS_KEY_ID"];
            secretAccessKey = Configuration["OutsideSystems:AWS_S3:SECRET_ACCESS_KEY"];
            var s3Config = new AmazonS3Config
            {
                ServiceURL = Configuration["OutsideSystems:AWS_S3:SERVICE_URL"],
                ForcePathStyle = true
            };

            Bucket = Configuration["OutsideSystems:AWS_S3:S3_BUCKET"];
            client = new AmazonS3Client(accessKeyId, secretAccessKey, s3Config);
        }
        public async Task<(bool check, string fullPath, object data)> Upload(string bucket, IFormFile file, string path)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(client);

                using var newMemoryStream = new MemoryStream();
                file.CopyTo(newMemoryStream);

                string fileName = DateTime.UtcNow.Ticks.ToString() + "_" + file.FileName.Replace(" ", string.Empty);

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
                return (false, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (false, null, null);
            }
        }

        public async Task<(byte[] data, string message, object data1)> GetFile(string fileFullName)
        {
            try
            {
                GetObjectRequest getObjectResponse = new GetObjectRequest()
                {
                    BucketName = Bucket,
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
                return (null, "File not found", null);
            }
        }

        public async Task<(bool check, string message)> DeleteFile(string fileFullName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = Bucket,
                    Key = fileFullName
                };

                Console.WriteLine("Deleting an object");
                await client.DeleteObjectAsync(deleteObjectRequest);
                return (true, "Success");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
                return (false, e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
                return (false, e.Message);
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
