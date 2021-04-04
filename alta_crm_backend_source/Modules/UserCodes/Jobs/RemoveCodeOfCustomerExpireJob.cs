using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Project.Modules.UserCodes.Services;
using Quartz;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UserCodes.Jobs
{
    public class RemoveCodeOfCustomerExpireJob : IJob
    {
        private readonly IServiceScopeFactory _servirceScopeFatory;
        public RemoveCodeOfCustomerExpireJob() { }
        public RemoveCodeOfCustomerExpireJob(IServiceScopeFactory servirceScopeFatory)
        {
            _servirceScopeFatory = servirceScopeFatory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("*******************************RemoveCodeOfCustomerExpireJob***********************************");
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IUserCodeService repositoryWrapperMariaDB = serviceScope.ServiceProvider.GetRequiredService<IUserCodeService>();

            repositoryWrapperMariaDB.RemoveCodeExpire();
            return Task.CompletedTask;
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<RemoveCodeOfCustomerExpireJob>();
        }
    }
}
