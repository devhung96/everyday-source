using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project.App.Databases;
using Project.App.DesignPatterns.ObserverPatterns;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Mappings;
using Project.App.Middlewares;
using Project.App.Schedules;
using Project.App.Swaggers;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Mappings;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using AutoMapper;
using Quartz;
using Project.Modules.Settings.Services;
using Project.Modules.DeviceTypes.Services;
using Project.Modules.Templates.Services;
using Project.Modules.TemplateDetails.Services;
using Project.Modules.Users.Services;
using Project.Modules.Users.Validations;
using Project.Modules.Devices.Services;
using Project.Modules.DeviceTypes.Services;
using Project.Modules.Groups.Sevices;
using Project.Modules.Medias.Services;
using Project.Modules.PlayLists.Services;
using Project.Modules.Schedules.Services;
using Project.Modules.Schedules.Validations;
using Project.Modules.Settings.Services;
using Project.Modules.TemplateDetails.Services;
using Project.Modules.Templates.Services;
using Project.Modules.Users.Services;
using Project.Modules.Users.Validations;
using Quartz;
using System.Text;
using Project.Modules.Groups.Sevices;
using Project.Modules.Medias.Services;
using Project.Modules.App.Services;
using Project.Modules.App.Jobs;
using Microsoft.AspNetCore.StaticFiles;
using Project.App.Kafka;

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
            services.UseQuartz(typeof(SendMqttDataChangeJob));
            #endregion

            #region Add Context Service
            ObserverPatternHandling.Instance.Connection(Configuration);
            services.AddDistributedMemoryCache();
            services.AddDbContextPool<MariaDBContext>(options => options.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]));
            services.AddSingleton<MongoDBContext>();

            #endregion

            #region Add Module Services

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IRepositoryWrapperMongoDB, RepositoryWrapperMongoDB>();
            services.AddScoped<IRepositoryWrapperMariaDB, RepositoryWrapperMariaDB>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ITemplateServices, TemplateServices>();
            services.AddScoped<ITemplateDetailServices, TemplateDetailServices>();


            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IDeviceTypeServices, DeviceTypeServices>();


            services.AddScoped<ISettingService, SettingService>();

            services.AddScoped<ISettingService, SettingService>();


            services.AddScoped<ISettingService, SettingService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IDeviceTypeServices, DeviceTypeServices>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IMediaTypeService, MediaTypeService>();

            services.AddScoped<IDeviceServices, DeviceServices>();



            services.AddScoped<IPlayListDetailService, PlayListDetailService>();
            services.AddScoped<IPlayListService, PlayListService>();

            services.AddTransient<IGroupService, GroupService>();



            services.AddTransient<IGroupService, GroupService>();
            services.AddScoped<ITemplateServices, TemplateServices>();
            services.AddScoped<ITemplateDetailServices, TemplateDetailServices>();



            services.AddScoped<IScheduleService, ScheduleService>();

            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IDeviceLogService, DeviceLogService>();

            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IDeviceTypeServices, DeviceTypeServices>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ITemplateServices, TemplateServices>();
            services.AddScoped<ITemplateDetailServices, TemplateDetailServices>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IDeviceServices, DeviceServices>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IPlayListDetailService, PlayListDetailService>();
            services.AddScoped<IPlayListService, PlayListService>();
            services.AddTransient<IGroupService, GroupService>();

            //services.AddScoped<ModuleService, ModuleService>();
            services.AddScoped<IPlayListDetailService, PlayListDetailService>();
            services.AddScoped<IPlayListService, PlayListService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddScoped<ITemplateServices, TemplateServices>();
            services.AddScoped<ITemplateDetailServices, TemplateDetailServices>();
            services.AddScoped<IPlayListDetailService, PlayListDetailService>();
            services.AddScoped<IPlayListService, PlayListService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddScoped<ITemplateServices, TemplateServices>();
            services.AddScoped<ITemplateDetailServices, TemplateDetailServices>();
            services.AddScoped<IAppSupportService, AppSupportService>();
            services.AddSingleton<Project.App.Mqtt.IMqttSingletonService, Project.App.Mqtt.MqttSingletonService>();
            services.AddSingleton<ISendScheduleService, SendScheduleService>();
            services.AddScoped<ITemplateShareService, TemplateShareService>();

            services.AddScoped<IMediaSupport, MediaSupport>();


            #endregion

            #region Add FluentValidation
            services.AddMvc()
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<StoreUserValidator>();
                    fv.RegisterValidatorsFromAssemblyContaining<ValidationStoreSchedule>();
                    fv.RegisterValidatorsFromAssemblyContaining<ValidationUpdateSchedule>();
                    fv.RegisterValidatorsFromAssemblyContaining<ValidationCalendar>();
                    fv.RegisterValidatorsFromAssemblyContaining<ValidationGetScheduleByDevice>();
                    fv.RegisterValidatorsFromAssemblyContaining<ValidationStoreScheduleNonDevice>();

                }).AddNewtonsoftJson(opts =>
                {
                    //opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
            #endregion

            #region Config swagger
            services.AddSwaggerDocumentation();
            #endregion
         
            #region Kafka 
            services.AddSingleton<KafkaClientHandle>();
            services.AddSingleton<KafkaProducer<string, string>>();
            services.AddHostedService<KafkaConsumer>();
            #endregion

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddControllers()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            services.AddControllers();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Issuer"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddAutoMapper(typeof(MappingProfile));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            #region Start Job
            IScheduler scheduler = app.ApplicationServices.GetService<IScheduler>();
            QuartzServicesUtilitie.StartJob<SampleJob>(scheduler, Configuration["SystemAction:SendMqttChangeDataJob"]);
            QuartzServicesUtilitie.StartJob<SendMqttDataChangeJob>(scheduler, Configuration["SystemAction:SendMqttChangeDataJob"]);
            #endregion


            #region Swagger
            app.UseSwaggerDocumentation();
            #endregion

            app.UseCors("AllowSpecificOrigin");


            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".m3u8"] = "application/x-mpegURL";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
