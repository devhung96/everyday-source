using Quartz;
using Quartz.Spi;
using System;

namespace Project.App.Schedules
{
    public class QuartzJonFactory : IJobFactory
    {
        private readonly IServiceProvider ServiceProvider;

        public QuartzJonFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            IJobDetail jobDetail = bundle.JobDetail;
            IJob job = (IJob)ServiceProvider.GetService(jobDetail.JobType);
            return job;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
