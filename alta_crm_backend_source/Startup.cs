using AutoMapper;
using Confluent.Kafka;
using FaceManagement;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Project.App.Databases;
using Project.App.DesignPatterns.ObserverPatterns;
using Project.App.Mappings;
using Project.App.Middlewares;
using Project.App.Schedules;
using Project.App.Schedules.Jobs;
using Project.App.Swagger;
using Project.Modules.Faces.FaceServices;
using Project.Modules.Groups.Services;
using Project.Modules.Kafka.Consumer;
using Project.Modules.Kafka.Producer;
using Project.Modules.Medias.Services;
using Project.Modules.OpenVidus.Services;
using Project.Modules.Ratings.Services;
using Project.Modules.Schedules.Models;
using Project.Modules.Schedules.Services;
using Project.Modules.Schedules.Validations;
using Project.Modules.Tags.Services;
using Project.Modules.Tags.Validation;
using Project.Modules.UserCodes.Jobs;
using Project.Modules.UserCodes.Services;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Service;
using Project.Modules.Users.UserKafka;
using Project.Modules.Users.Validations;
using Project.Modules.UsersModes.Services;
using Quartz;
using RepositoriesManagement;
using Repository;
using System;
using System.Text;

namespace Project
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationStatic = configuration;
        }
        public static IConfiguration ConfigurationStatic { get; private set; }
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
            services.UseQuartz(typeof(RemoveCodeOfCustomerExpireJob));
            #endregion

            #region Add Context Service
            services.AddDbContextPool<MariaDBContext>(options => options.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]));
            services.AddSingleton<MongoDBContext>();
            services.AddSingleton<RedisDBContext>();
            #endregion

            #region Add Helper Service
            //services.AddScoped<ISortHelper<Customer>, SortHelper<Customer>>();
            #endregion

            #region Add Module Services
            services.AddScoped<IRepositoryWrapperMariaDB, RepositoryWrapperMariaDB>();
            services.AddScoped<IRepositoryMongoWrapper, RepositoryMongoWrapper>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IViduService, ViduService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFaceService, FaceService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<ILogImportSevice, LogSevice>();
            services.AddScoped<HandleTaskSchedule<RegisterUserResponse>>();
            services.AddScoped<ISupportUserService, SupportUserService>();
            services.AddScoped<IMeetingScheduleService, MeetingScheduleService>();
            services.AddScoped<HelperService>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddSingleton<IFaceGrpcService, FaceGrpcService>();

            services.AddScoped<ITicketTypeService, TicketTypeService>();
            services.AddScoped<ITicketTypeKafkaService, TicketTypeKafkaService>();


            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITagKafkaService, TagKafkaService>();


            services.AddScoped<IUserCodeService, UserCodeService>();




            #endregion

            #region Add Validation
            services.AddMvc()
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    fv.RegisterValidatorsFromAssemblyContaining<StoreScheduleEmployeeValidation>();
                    fv.RegisterValidatorsFromAssemblyContaining<ImportScheduleCustomerValidation>();
                    fv.RegisterValidatorsFromAssemblyContaining<LoginFaceValidation>();
                    fv.RegisterValidatorsFromAssemblyContaining<StoreScheduleCustomerValidation>();
                    fv.RegisterValidatorsFromAssemblyContaining<GetScheduleValidation>();
                    fv.RegisterValidatorsFromAssemblyContaining<ImportEmployeeValidation>();

                    #region Tag
                    fv.RegisterValidatorsFromAssemblyContaining<InsertTagValidation>();
                    #endregion
                });
            #endregion

            ObserverPatternHandling.Instance.Connection(Configuration);

            #region Kafka

            services.AddSingleton<KafkaClientHandle>();
            services.AddSingleton<KafkaDependentProducer<string, string>>();
            services.AddSingleton<KafkaDependentProducer<Null, string>>();
            services.AddHostedService<RequestTimeConsumer>();
            services.AddSingleton<HandleTask<RegisterFace>>();
            services.AddSingleton<HandleTask<DetectFace>>();
            services.AddSingleton<HandleTask<object>>();
            services.AddSingleton<HandleTask<DeleteFace>>();
            services.AddSingleton<HandleTaskSchedule<RegisterUserResponse>>();


            
            #endregion

            #region Authorization
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["TokenSettings:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            #endregion

            #region AutoMapper and Controller
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddControllers()
                   .AddNewtonsoftJson(options =>
                   {
                       // Use the default property (Pascal) casing\
                       options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                       options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                   });
            #endregion

            #region Grpc
            services.AddGrpcClient < FaceManagementService.FaceManagementServiceClient> (o =>
               {
                   o.Address = new Uri(Configuration.GetSection("OutsideSystems:FaceSettings:FaceGrpcUrl").Get<string>());
               });
            services.AddGrpcClient<RepositoryManagement.RepositoryManagementClient>(o =>
            {
                o.Address = new Uri(Configuration.GetSection("OutsideSystems:FaceSettings:FaceGrpcUrl").Get<string>());
            });
            services.AddGrpcClient <SurveillanceManagement.SurveillanceManagement.SurveillanceManagementClient> (o =>
               {
                   o.Address = new Uri(Configuration.GetSection("OutsideSystems:FaceSettings:FaceGrpcUrl").Get<string>());
               });
            #endregion

            #region Config swagger
            services.AddSwaggerDocumentation();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            #region Start Job
            IScheduler scheduler = app.ApplicationServices.GetService<IScheduler>();
            QuartzServicesUtilitie.StartJob<SampleJob>(scheduler, Configuration["SystemAction:SampleJob"]);
            QuartzServicesUtilitie.StartJob<RemoveCodeOfCustomerExpireJob>(scheduler, Configuration["SystemAction:RemoveCodeOfCustomerExpireJob"]);
            #endregion

            
            #region Swagger
            app.UseSwaggerDocumentation();
            #endregion
           

            app.UseCors("AllowSpecificOrigin");

            app.UseStaticFiles();

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
