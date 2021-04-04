using Confluent.Kafka;
using FaceManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Kafka;
using Project.Modules.Aws.Services;
using Project.Modules.Detections.Models;
using Project.Modules.Logs.Requests;
using Project.Modules.Logs.Services;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.TicketDevices.Entities;
using SurveillanceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static SurveillanceManagement.SurveillanceManagement;

namespace Project.Modules.Detections.Services
{
    public interface IDetectService
    {
        (object data, bool check, string message) Detect(string deviceId, string modeId, string keyCode, IFormFile image);
    }

    public class DetectService : IDetectService
    {
        private readonly IConfiguration Configuration;
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        private readonly IAwsService AwsService;
        private string Topic;
        private readonly KafkaProducer<string, string> Producer;
        private readonly HandleTask<DetectFace> HandleTaskDetect;
        private readonly string App;
        private readonly string BucketDetect;
        private readonly string BucketRegister;
        private readonly ILogService LogService;
        private readonly int TimeOut;
        private readonly FaceManagementService.FaceManagementServiceClient FaceManagementServiceClient;
        private readonly SurveillanceManagementClient SurveillanceManagementServiceClient;

        public DetectService(IConfiguration configuration, IRepositoryWrapperMariaDB repositoryWrapperMariaDB, IAwsService awsService, KafkaProducer<string, string> producer, HandleTask<DetectFace> handleTaskDetect, ILogService logService, FaceManagementService.FaceManagementServiceClient faceManagementServiceClient, SurveillanceManagementClient surveillanceManagementServiceClient)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;
            AwsService = awsService;
            Configuration = configuration;
            Producer = producer;
            HandleTaskDetect = handleTaskDetect;
            App = "TEST.wifi";
            BucketDetect = Configuration["OutsideSystems:AWS_S3:S3_BUCKET_DETECT"];
            BucketRegister = Configuration["OutsideSystems:AWS_S3:S3_BUCKET_REGISTER"];
            LogService = logService;
            TimeOut = int.Parse(Configuration["OutsideSystems:Kafka:TimeoutCache"]);
            FaceManagementServiceClient = faceManagementServiceClient;
            SurveillanceManagementServiceClient = surveillanceManagementServiceClient;
        }

        public (object data, bool check, string message) Detect(string deviceId, string modeId, string keyCode, IFormFile image)
        {
            DateTime now = DateTime.UtcNow;
            string dayOfYear = $"{now.Day}/{now.Month}";
            string dayOffWeek = now.DayOfWeek.ToString().Substring(0, 3);
            TimeSpan todayTime = now.TimeOfDay;
            string faceId = "";

            StoreLogRequest logRequest = new StoreLogRequest
            {
                DeviceId = deviceId,
                LogName = "Device user authentication",
                LogAccess = GetModeName(modeId),
                LogAccessTime = now,
                LogStatus = Logs.Entities.LogStatus.Fail
            };

            TimeSpan timeSpanUTCNow = now.TimeOfDay;

            List<string> ticketIds = RepositoryWrapperMariaDB.TicketTypeDevices.FindByCondition(x => x.DeviceId.Equals(deviceId)).Select(x => x.TicketTypeId).ToList();

            List<RegisterDetect> registerDetects = RepositoryWrapperMariaDB.RegisterDetects.FindByCondition(x => x.ModeId.Equals(modeId) && ticketIds.Contains(x.TicketTypeId)).ToList();

            if (registerDetects.Count == 0)
            {
                logRequest.LogMessage = "Nothing Tag Contains This Mode";
                LogService.Store(logRequest);
                return (null, false, "NothingTagContainsThisMode");
            }



            if (modeId.Equals("Face_ID")) // Face-ID
            {
                #region Grpc_Detect_Face

                var uploadImage = AwsService.Upload(BucketDetect, image, DateTime.UtcNow.ToString("dd-MM-yyyy")).Result;
                if (!uploadImage.check)
                {
                    logRequest.LogMessage = "UploadToS3Fail";
                    logRequest.LogStatus = Logs.Entities.LogStatus.Fail;
                    LogService.Store(logRequest);
                    return (null, false, uploadImage.data?.ToString());
                }
                string objectName = uploadImage.fullPath;

                FaceSurveillance faceSurveillance = new FaceSurveillance
                {
                    Bucket = BucketDetect,
                    ObjectName = objectName,
                    Surveillance = Configuration["OutsideSystems:FaceSettings:SurveillanceId"]
                };

                try
                {
                    SurveillanceManagement.FaceResponses faceResponses = SurveillanceManagementServiceClient.detect(faceSurveillance);
                    SurveillanceManagement.FaceResponse result = faceResponses.Faces.FirstOrDefault();
                    faceId = result.Id;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("InvalidArgument"))
                    {
                        return (null, false, "ImageCannotBeRecognized");
                    }
                    return (null, false, ex.Message.Replace(" ", string.Empty));
                }


                #endregion

                keyCode = faceId;
            }

            registerDetects = registerDetects.Where(x => x.RgDectectKey.Equals(keyCode)).ToList();

            if (registerDetects.Count == 0)
            {
                return (null, false, "KeyCodeError");
            }

            string userId = registerDetects.FirstOrDefault(x => x.RgDectectKey.Equals(keyCode)).RgDectectUserId;

            (string getProfileUser, int? statusCode) = HttpMethod.Get.SendRequestWithStringContent($"{Configuration["OutsideSystems:CRM:URL"]}/api/users/{userId}");

            DetectionResponse detectionResponse = new DetectionResponse();

            if (statusCode == 200)
            {
                JObject getProfileUserData = JObject.Parse(getProfileUser);
                detectionResponse.UserId = userId;
                detectionResponse.FirstName = getProfileUserData["data"]["userFirstName"]?.ToString();
                detectionResponse.LastName = getProfileUserData["data"]["userLastName"]?.ToString();
                detectionResponse.Images = JsonConvert.DeserializeObject<List<string>>(getProfileUserData["data"]["userImages"]?.ToString());
                logRequest.FirstName = getProfileUserData["data"]["userFirstName"]?.ToString();
                logRequest.LastName = getProfileUserData["data"]["userLastName"]?.ToString();
            }

           

            foreach (var registerDetect in registerDetects)
            {
                List<RegisterDetectDetail> registerDetectDetails = RepositoryWrapperMariaDB.RegisterDetectDetails.FindByCondition(x =>
                x.RegisterDetectId.Equals(registerDetect.RegisterDetectId)
                && (x.RgDectectDetailDateBegin <= now && (!x.RgDectectDetailDateEnd.HasValue || x.RgDectectDetailDateEnd.Value >= now))
                && (x.RgDectectDetailTimeBegin <= timeSpanUTCNow && x.RgDectectDetailTimeEnd >= timeSpanUTCNow))
                .ToList();

                JObject extension = !string.IsNullOrEmpty(registerDetect.RgDectectExtension) ? JObject.Parse(registerDetect.RgDectectExtension) : new JObject();

                foreach (var registerDetectDetail in registerDetectDetails)
                {
                    if (registerDetectDetail.RgDectectDetailRepeat == RepeatType.RepeatWeek && registerDetectDetail.RgDectectDetailRepeatValueData.Contains(dayOffWeek))
                    {
                        logRequest.LogMessage = "Success";
                        logRequest.LogStatus = Logs.Entities.LogStatus.Success;
                        logRequest.UserId = registerDetect.RgDectectUserId;
                        LogService.Store(logRequest);
                        return (detectionResponse, true, "WellcomeToHDBank");
                    }

                    else if (registerDetectDetail.RgDectectDetailRepeat == RepeatType.RepeatMonth && registerDetectDetail.RgDectectDetailRepeatValueData.Contains(now.Day.ToString()))
                    {
                        logRequest.LogMessage = "Success";
                        logRequest.LogStatus = Logs.Entities.LogStatus.Success;
                        logRequest.UserId = registerDetect.RgDectectUserId;
                        LogService.Store(logRequest);
                        return (detectionResponse, true, "WellcomeToHDBank");
                    }

                    else if (registerDetectDetail.RgDectectDetailRepeat == RepeatType.RepeatYear && registerDetectDetail.RgDectectDetailRepeatValueData.Contains(dayOfYear))
                    {
                        logRequest.LogMessage = "Success";
                        logRequest.LogStatus = Logs.Entities.LogStatus.Success;
                        logRequest.UserId = registerDetect.RgDectectUserId;
                        LogService.Store(logRequest);
                        return (detectionResponse, true, "WellcomeToHDBank");
                    }

                    else
                    {
                        logRequest.LogMessage = "Success";
                        logRequest.LogStatus = Logs.Entities.LogStatus.Success;
                        logRequest.UserId = registerDetect.RgDectectUserId;
                        LogService.Store(logRequest);
                        return (detectionResponse, true, "WellcomeToHDBank");
                    }
                }
            }
            logRequest.LogMessage = "Fail";
            LogService.Store(logRequest);
            return (null, false, "YouDoNotHaveAccess");
        }

        public string GetModeName(string modeId)
        {
            return RepositoryWrapperMariaDB.Modes.FindByCondition(x => x.ModeId.Equals(modeId)).FirstOrDefault()?.ModeName;
        }

        private void deliveryReportHandleString(DeliveryReport<string, string> deliveryReport)
        {
            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                Console.WriteLine($"Message delivery failed: {deliveryReport.Message.Value}");
            }
        }
    }
}
