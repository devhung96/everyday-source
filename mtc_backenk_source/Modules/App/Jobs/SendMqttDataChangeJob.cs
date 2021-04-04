using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Mqtt;
using Project.Modules.App.Entities;
using Project.Modules.App.Services;
using Project.Modules.Devices.Entities;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.App.Jobs
{
    public class SendMqttDataChangeJob : IJob
    {
        private readonly IServiceScopeFactory _servirceScopeFatory;

        public SendMqttDataChangeJob() { }

        public SendMqttDataChangeJob(IServiceScopeFactory servirceScopeFatory)
        {
            _servirceScopeFatory = servirceScopeFatory;

        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("******************SendMqttDataChangeJob");
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            ISendScheduleService sendScheduleService = serviceScope.ServiceProvider.GetRequiredService<ISendScheduleService>();
            sendScheduleService.SenMqttSchedule();
            return Task.CompletedTask;
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<SendMqttDataChangeJob>();
        }
    }
}
