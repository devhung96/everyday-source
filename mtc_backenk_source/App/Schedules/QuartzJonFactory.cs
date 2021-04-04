﻿using Quartz;
using Quartz.Spi;
using System;

namespace Project.App.Schedules
{
    public class QuartzJonFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QuartzJonFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;

            var job = (IJob)_serviceProvider.GetService(jobDetail.JobType);
            return job;
        }

        public void ReturnJob(IJob job) { }
    }
}
