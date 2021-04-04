using Microsoft.AspNetCore.Builder;
using Quartz;
using System.Threading.Tasks;

namespace Project.App.Schedules.Jobs
{
    public class SampleJob : IJob
    {
        public SampleJob() { }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<SampleJob>();
        }
    }
}
