using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Quartz;
using System.Threading.Tasks;

namespace Project.App.Schedules
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
