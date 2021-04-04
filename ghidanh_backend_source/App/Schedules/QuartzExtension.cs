
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Linq;
using Quartz;

namespace Project.App.Schedules
{
    public static class QuartzExtension
    {
        public static void UseQuartz(this IServiceCollection services, params Type[] jobs)
        {
            services.AddSingleton<IJobFactory, QuartzJonFactory>();
            services.Add(jobs.Select(jobType => new ServiceDescriptor(jobType, jobType, ServiceLifetime.Singleton)));

            services.AddSingleton<IScheduler>(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();
                return scheduler;
            });
        }
    }
}
