using Common;
using FaceManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Requests;
using Project.Modules.Users.UserKafka;
using Project.Modules.UsersModes.Entities;
using Repository;
using SurveillanceManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Faces.FaceServices
{
    public interface IFaceGrpcService
    {
        (RegisterFace data, string message) RegisterFace(IFormFile imageFace, string externalId, string repositoryId, bool train = false);
        (DetectFace data, string message) DetectFace(IFormFile imageFace);
        (bool result, string message) RemoveFaceByExternal(List<UserMode> listUserMode);
    }
    public class FaceGrpcService : IFaceGrpcService
    {
        private readonly IConfiguration _configuration;
        private readonly string _bucketFaceRegister;
        private readonly string _bucketFaceDetect;
       

        private readonly IMediaService _mediaService;
        private readonly FaceManagementService.FaceManagementServiceClient _faceManagementServiceClient;
        private readonly SurveillanceManagement.SurveillanceManagement.SurveillanceManagementClient _surveillanceManagementClient;
        private readonly IRepositoryWrapperMariaDB _repositoryWrapperMariaDB;


        public FaceGrpcService(SurveillanceManagement.SurveillanceManagement.SurveillanceManagementClient surveillanceManagementClient ,  IRepositoryWrapperMariaDB repositoryWrapperMariaDB,IMediaService mediaService, IConfiguration configuration, FaceManagementService.FaceManagementServiceClient faceManagementServiceClient)
        {
            
            _configuration = configuration;
            _bucketFaceRegister = _configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE_REGISTER"];
            _bucketFaceDetect = _configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE-DETECT"];


            _mediaService = mediaService;
            _faceManagementServiceClient = faceManagementServiceClient;
            _repositoryWrapperMariaDB = repositoryWrapperMariaDB;
            _surveillanceManagementClient = surveillanceManagementClient;
        }

        public (RegisterFace data, string message) RegisterFace(IFormFile imageFace, string externalId, string repositoryId,bool train = false)
        {
            
            var image = _mediaService.UploadFileAWS(_bucketFaceRegister, imageFace, DateTime.UtcNow.Date.ToString("dd-MM-yyyy")).Result;
            if (!image.check)
            {
                return (null, image.data.ToString());
            }
            string objectName = image.fullPath;

            FaceRequest faceRequest = new FaceRequest
            {
                Repository = repositoryId,
                Bucket = _bucketFaceRegister,
                ObjectName = objectName,
                ExternalId = externalId,
                Train = train
            };

            try
            {
                FaceManagement.FaceResponse result = _faceManagementServiceClient.register(faceRequest);
                RegisterFace registerFace = new RegisterFace
                {
                    Id = result.Id,
                    FullPath = objectName,
                    Message = result.Message,
                    Result = true,
                    Status = 200
                };
                return (registerFace, "Success");
            }
            catch (Exception ex)
            {
                return (null, "There was an error with the face protos system : " + ex.Message);
            }
        }

        public (DetectFace data, string message) DetectFace(IFormFile imageFace)
        {
            var timer = new Stopwatch();
            timer.Start();
            var image = _mediaService.UploadFileAWS(_bucketFaceDetect, imageFace, DateTime.UtcNow.Date.ToString("dd-MM-yyyy")).Result;
            if (!image.check)
            {
                return (null, image.data.ToString());
            }
            string objectName = image.fullPath;

            SurveillanceManagement.FaceSurveillance faceRequest = new FaceSurveillance
            {
                Bucket = _bucketFaceDetect,
                ObjectName = objectName,
                Surveillance = _configuration["OutsideSystems:FaceSettings:SurveillanceId"]
            };
            timer.Stop();
            Console.WriteLine("***********Detect S3: " + timer.Elapsed.ToString(@"m\:ss\.fff"));

            var timer1 = new Stopwatch();
            timer1.Start();
            try
            {
                SurveillanceManagement.FaceResponses faceResponses = _surveillanceManagementClient.detect(faceRequest);
                SurveillanceManagement.FaceResponse result = faceResponses.Faces.FirstOrDefault();
                DetectFace detectFace = new DetectFace
                {
                    Id = result?.Id,
                    Message = faceResponses.Message,
                    Result = true,
                    Status = 200,
                    ExtenId = result.ExternalId
                };
                if (string.IsNullOrEmpty(detectFace.Id)) return (null, "FaceError");
                timer1.Stop();
                Console.WriteLine("***********Detect Grpc: " + timer1.Elapsed.ToString(@"m\:ss\.fff"));
                return (detectFace, "Success");

            }
            catch (Exception ex)
            {
                timer1.Stop();
                Console.WriteLine("***********Detect Grpc: " + timer1.Elapsed.ToString(@"m\:ss\.fff"));
                return (null, "There was an error with the face protos system : " + ex.Message);
            }


        }

        public (bool result, string message) RemoveFaceByExternal(List<UserMode> listUserMode)
        {
            if(listUserMode.Count == 0) return (true, "RemoveSuccess");
            FaceRemoveExternalRequest faceRemoveExternalRequest = new FaceRemoveExternalRequest
            {
                ExternalId = listUserMode.FirstOrDefault().UserId,
                Repository = listUserMode.FirstOrDefault().RepositoryId is null ? _configuration["OutsideSystems:FaceSettings:App"] : listUserMode.FirstOrDefault().RepositoryId,
            };
            try
            {
                HTTP result = _faceManagementServiceClient.remove_external_id(faceRemoveExternalRequest);
                if (result.Status == 200) return (true, "RemoveSuccess");
                return (false, result.Message);
            }
            catch (Exception ex)
            {
                return (false, "There was an error with the face protos system : " + ex.Message);
            }

        }

    }
}
