using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Mqtt;
using Project.Modules.App.Entities;
using Project.Modules.App.Services;
using Project.Modules.Devices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.App.Jobs
{
    public interface ISendScheduleService
    {
        public void SenMqttSchedule();
    }
    public class SendScheduleService: ISendScheduleService
    {
        private readonly IServiceScopeFactory _servirceScopeFatory;

        private readonly IAppSupportService _appSupportService;
        private readonly IMqttSingletonService _mqttSingletonService;

        public SendScheduleService(IServiceScopeFactory servirceScopeFatory, IAppSupportService appSupportService, IMqttSingletonService mqttSingletonService)
        {
            _appSupportService = appSupportService;
            _mqttSingletonService = mqttSingletonService;

            _servirceScopeFatory = servirceScopeFatory;
        }

        public void SenMqttSchedule()
        {
            var dateTimeNow = DateTime.Now;

            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB _repositoryWrapperMariaDB = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            List<DeviceResponse> devices = _repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceStatus == DEVICESTATUS.ACTIVED && x.DeviceExpired > dateTimeNow).Select(x => new DeviceResponse(x, null,null)).ToList();


            List<DataScheduleTmp> dataScheduleTmps = _repositoryWrapperMariaDB.DataScheduleTmps.FindByCondition(x => devices.Select(y => y.DeviceId).Contains(x.DataScheduleTmpKey)).ToList();

            foreach (var device in devices)
            {
                DateTime tmpDateTimeNow = DateTime.UtcNow.Date;



                (List<ScheduleResponse> data, string message) = _appSupportService.GetScheduleByTime(
                        new Requests.GetScheduleByTimeRequest
                        {
                            timeBegin = tmpDateTimeNow,
                            timeEnd = tmpDateTimeNow.AddHours(24).AddTicks(-1)
                        },
                        deviceId: device.DeviceId
                    );


                var tmpDataSchedule = dataScheduleTmps.FirstOrDefault(x => x.DataScheduleTmpKey.Equals(device.DeviceId));
                if (tmpDataSchedule is null)
                {
                    _repositoryWrapperMariaDB.DataScheduleTmps.Add(new DataScheduleTmp
                    {
                        DataScheduleTmpKey = device.DeviceId,
                        DataScheduleTmpData = JsonConvert.SerializeObject(data)
                    });
                    _repositoryWrapperMariaDB.SaveChanges();
                    continue;
                }

                if (JsonConvert.SerializeObject(data).Equals(tmpDataSchedule.DataScheduleTmpData)) { continue; }

                tmpDataSchedule.DataScheduleTmpData = JsonConvert.SerializeObject(data);
                _repositoryWrapperMariaDB.SaveChanges();

                Console.WriteLine($"********************:{TopicDefine.SCHEDULE_DEVICE}/{device.DeviceId}");
                _mqttSingletonService.PingMessage($"{TopicDefine.SCHEDULE_DEVICE}/{device.DeviceId}", "", false);
                continue;
            }
        }
    }
}
