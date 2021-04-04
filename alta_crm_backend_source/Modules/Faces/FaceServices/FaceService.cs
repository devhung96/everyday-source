using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Project.Modules.Kafka.Producer;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Requests;
using Project.Modules.Users.UserKafka;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Modules.Faces.FaceServices
{
    public interface IFaceService
    {
        (DetectFace data, string message) DetectFace(IFormFile imageFace);
        (RegisterFace data, string message) RegisterFace(IFormFile imageFace);
    }
    public class FaceService : IFaceService
    {
        private readonly IMediaService mediaService;
        private readonly string App;
        private readonly string BucketFaceRegister;
        private readonly string BucketFaceDetect;
        private readonly IConfiguration configuration;
        private readonly HandleTask<DetectFace> HandleTaskDetectFace;
        private readonly HandleTask<RegisterFace> HandleTaskRegister;
        private readonly HandleTask<DeleteFace> HandleTaskDelete;
        KafkaDependentProducer<string, string> Producer;
        private readonly int WaitingKafka;

        public FaceService(
                IMediaService _mediaService,
                IConfiguration _configuration,
                HandleTask<DetectFace> _HandleTaskDetectFace,
                KafkaDependentProducer<string, string> _Producer,
                HandleTask<RegisterFace> _HandleTaskRegister,
                HandleTask<DeleteFace> _HandleTaskDelete
            )
        {
            configuration = _configuration;
            mediaService = _mediaService;
            App = configuration["OutsideSystems:FaceSettings:App"];
            BucketFaceRegister = configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE_REGISTER"];
            BucketFaceDetect = configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE-DETECT"];
            HandleTaskDetectFace = _HandleTaskDetectFace;
            HandleTaskRegister = _HandleTaskRegister;
            HandleTaskDelete = _HandleTaskDelete;
            Producer = _Producer;
            WaitingKafka = int.Parse(configuration["Kafka:WaitingKafka"]);
        }

        public (DetectFace data, string message) DetectFace(IFormFile imageFace)
        {
            var image = mediaService.UploadFileAWS(BucketFaceDetect, imageFace, DateTime.UtcNow.Date.ToString("dd-MM-yyyy")).Result;
            if (!image.check)
            {
                return (null, image.data.ToString());
            }
            string objectName = image.fullPath;
            JObject jObject = JObject.FromObject(new { bucket = BucketFaceDetect, object_name = objectName, app = App });
            string key = $"{App}.{BucketFaceDetect}.{objectName}";
            HandleTaskDetectFace.HandleTasks.Add(key, false);
            Message<string, string> message = new Message<string, string> { Key = "data", Value = jObject.ToString() };
            this.Producer.Produce("DETECT_FACE", message, deliveryReportHandleString);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(int.Parse(configuration["Kafka:TimeoutCache"])));

            while (!HandleTaskDetectFace.Get(key) && !cancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine("Detect Task Running");
                Thread.Sleep(WaitingKafka);
            }

            var data = HandleTaskDetectFace.GetData(key);
            HandleTaskDetectFace.Remove(key);

            if (cancellationTokenSource.IsCancellationRequested)
            {
                return (null, "TimeOut");
            }
            if (data.Status != 200)
            {
                return (null, data.Message);
            }
            return (data, data.Message);
        }

        public (RegisterFace data, string message) RegisterFace(IFormFile imageFace)
        {
            var image = mediaService.UploadFileAWS(BucketFaceRegister, imageFace, DateTime.UtcNow.Date.ToString("dd-MM-yyyy")).Result;
            if (!image.check)
            {
                return (null, image.data.ToString());
            }
            string objectName = image.fullPath;
            JObject jObject = JObject.FromObject(new { bucket = BucketFaceRegister, object_name = objectName, app = App });
            string key = $"{App}.{BucketFaceRegister}.{objectName}";
            HandleTaskRegister.HandleTasks.Add(key, false);
            Message<string, string> message = new Message<string, string> { Key = "data", Value = jObject.ToString() };
            this.Producer.Produce("REGISTER_FACE", message, deliveryReportHandleString);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

            while (!HandleTaskRegister.Get(key) && !cancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine("REGISTER_FACE Task Running");
                Thread.Sleep(WaitingKafka);
            }

            var data = HandleTaskRegister.GetData(key);
            HandleTaskRegister.Remove(key);

            if (cancellationTokenSource.IsCancellationRequested)
            {
                return (null, "TimeOut");
            }
            if (data.Status != 200)
            {
                if (data.Message.Equals("FACE_DUPLICATE"))
                {
                    data.FullPath = objectName;
                    return (data, data.Message);
                }
                return (null, data.Message);
            }
            data.FullPath = objectName;
            return (data, data.Message);
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
