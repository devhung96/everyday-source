using Quartz;

namespace Project.App.Schedules
{
    public static class QuartzServicesUtilitie
    {
        public static void StartJob<TJob>(IScheduler scheduler, string cron)
        where TJob : IJob
        {
            var jobName = typeof(TJob).FullName;

            var job = JobBuilder.Create<TJob>()
                .WithIdentity(jobName)
                .Build();

            var trigger = TriggerBuilder.Create()
             .WithIdentity($"{jobName}.trigger")
             .StartNow()
             .WithCronSchedule(cron)
             .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
