using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.App.Databases;
using Project.App.Middlewares;
using Project.App.Schedules;
using Project.App.Swaggers;
using Quartz;
using FluentValidation.AspNetCore;
using Project.App.Kafka;
using Project.Modules.Devices.Validates;
using Project.Modules.Devices.Services;
using Project.Modules.DeviceTypes.Services;
using Project.Modules.Logs.Services;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Mappings;
using AutoMapper;
using Project.Modules.RegisterDetects.Services;
using Project.Modules.Detections.Services;
using Project.Modules.Aws.Services;
using Project.Modules.Detections.Models;
using Project.Modules.RegisterDetects.Validations;
using Project.Modules.Tags.Services;
using Project.Modules.Tags.Validations;
using Project.Modules.TicketDevices.Services;
using Project.Modules.TicketDevices.Validations;
using Project.Modules.Tickets.Services;
using Project.Modules.Detections.Validations;
using Newtonsoft.Json.Converters;
using FaceManagement;
using RepositoriesManagement;
using System;

namespace Project
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Add Cors Service
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(Configuration.GetSection($"AllowedOrigins").Get<string[]>())
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            #endregion

            #region Add Cron Job Service
            services.UseQuartz(typeof(SampleJob));
            #endregion

            #region Add Context Service
            services.AddDbContextPool<MariaDBContext>(options => options.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]));
            services.AddSingleton<MongoDBContext>();
            services.AddSingleton<RedisDBContext>();
            #endregion

            #region Add Module Services

            services.AddScoped<IRepositoryWrapperMongoDB, RepositoryWrapperMongoDB>();
            services.AddScoped<IRepositoryWrapperMariaDB, RepositoryWrapperMariaDB>();

            services.AddSingleton<KafkaClientHandle>();
            services.AddSingleton<KafkaProducer<string, string>>();
            services.AddHostedService<KafkaConsumer>();

            services.AddScoped<IDetectService, DetectService>();
            services.AddScoped<IAwsService, AwsService>();
           
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IAppLogService, AppLogService>();
            services.AddScoped<IRegisterDetectService, RegisterDetectService>();
            services.AddScoped<IRegisterDetectKafkaService, RegisterDetectKafkaService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IDeviceTypeService, DeviceTypeService>();
            services.AddSingleton<HandleTask<DetectFace>>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITicketDeviceService, TicketDeviceService>();
            services.AddScoped<ITicketTypeService, TicketTypeService>();
            services.AddScoped<ITagKafkaService, TagKafkaService>();
         
            #endregion

            #region Add FluentValidation
            services.AddMvc()
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<AddDeviceValidator>();
                    fv.RegisterValidatorsFromAssemblyContaining<UpdateDeviceValidator>();
                    fv.RegisterValidatorsFromAssemblyContaining<UpdateTagValidator>();
                    fv.RegisterValidatorsFromAssemblyContaining<AddTagDeviceValidator>();
                    fv.RegisterValidatorsFromAssemblyContaining<DetectValidate>();
                }).AddNewtonsoftJson(opts =>
                {
                    //opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                }) ;
            #endregion

            #region Config swagger
            services.AddSwaggerDocumentation();
            #endregion

            #region Grpc
            services.AddGrpcClient<FaceManagementService.FaceManagementServiceClient>(o =>
            {
                o.Address = new Uri(Configuration.GetSection("OutsideSystems:FaceSettings:FaceGrpcUrl").Get<string>());
            });
            services.AddGrpcClient<SurveillanceManagement.SurveillanceManagement.SurveillanceManagementClient>(o =>
            {
                o.Address = new Uri(Configuration.GetSection("OutsideSystems:FaceSettings:FaceGrpcUrl").Get<string>());
            });

            services.AddGrpcClient<RepositoryManagement.RepositoryManagementClient>(o =>
            {
                o.Address = new Uri(Configuration.GetSection("OutsideSystems:FaceSettings:FaceGrpcUrl").Get<string>());
            });
            #endregion
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            #region Start Job
            IScheduler scheduler = app.ApplicationServices.GetService<IScheduler>();
            // QuartzServicesUtilitie.StartJob<SampleJob>(scheduler, Configuration["SystemAction:SummarizeRatingJob"]);
            #endregion


            #region Swagger
            app.UseSwaggerDocumentation();
            #endregion


            app.UseCors("AllowSpecificOrigin");

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
