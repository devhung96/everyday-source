using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Project.App.Database;
using Project.App.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Project.Modules.Medias.Services;
using Quartz.Spi;
using Project.App.Schedules;
using Quartz;
using Quartz.Impl;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Project.Modules.Users.Services;
using Project.Modules.WhitelistIps.Services;
//using Project.Modules.Surveys.Services;
using Project.Modules.Authorities.Services;
using Project.App.Providers;
using Project.Modules.PermissionGroups.Services;
using Project.Modules.Permissions.Services;
using Project.Modules.PermissonUsers.Services;
using Project.Modules.Groups.Services;
using Project.Modules.Organizes.Services;
using AutoMapper;
using Project.App.Helpers;
using Project.Modules.Parameters.Services;
using Project.Modules.Invitations.Service;
using Project.Modules.Question.Services;
using Project.Modules.Sessions.Services;
using Project.Modules.Events.Services;
using Project.Modules.Functions.Services;
using Project.Modules.Chats.Services;
using Project.Modules.Users.Caches;
using Project.Modules.Documents.Services;
using Project.Modules.PermissionOrganizes.Services;
using Project.Modules.Reports.Services;
using Project.Modules.GroupOrganizes.Services;
using Project.Modules.Question.Validation;
using Microsoft.AspNetCore.Http;

namespace Project
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );

            });
            services.AddControllersWithViews();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            #region Transport Pattern Provider
            TransportPatternHandlingProvider.Instance.Connection(Configuration);
            #endregion


            services.AddDistributedMemoryCache();
            services.AddMvc().AddNewtonsoftJson();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidIssuer = Configuration["Jwt:Issuer"].ToString(),
                    ValidAudience = Configuration["Jwt:Issuer"].ToString(),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"].ToString()))
                };

            });

            services.AddDbContextPool<MariaDBContext>(options => options.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]));
            services.AddSingleton<MongoDBContext>();
            services.AddSingleton<RedisDBContext>();

            // job
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzHostedService>();

            #region Module Services
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUseSuperService, UserSuperService>();
            services.AddScoped<IUserSupportService, UserSupportService>();
            services.AddScoped<IPerUserServices, PerUserServices>();
            services.AddScoped<IPermissionServices, PermissionServices>();
            services.AddScoped<IPermissionGroupServices, PermissionGroupServices>();
            services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
            services.AddScoped<IWhiteListIpService, WhiteListIpService>();
            services.AddScoped<IPermissionOrganizeService, PermissionOrganizeService>();
            //services.AddScoped<ISurveyUserServices, SurveyUserServices>();
            //services.AddScoped<ISurveyServices, SurveyServices>();
            services.AddScoped<IAuthorityService, AuthorityService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupOrganizeService, GroupOrganizeService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IFunctionService, FunctionService>();
            services.AddScoped<IOrganizeService, OrganizeService>();
            services.AddScoped<IEventUserService, EventUserService>();
            services.AddScoped<IEventService, EventService>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IParameterService, ParameterService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<IQuestionAnswersService, QuestionAnswersService>();
            services.AddScoped<ISubmitQuestionService, SubmitQuestionService>();
            services.AddScoped<IMediaShareholderService, MediaShareholderService>();
            services.AddScoped<IManagementOTPService, ManagementOTPService>();
            services.AddScoped<IManagementAccountAdminService, ManagementAccountAdminService>();
            services.AddScoped<IMediaFolderService, MediaFolderService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IOperatorService, OperatorService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddHttpContextAccessor();
            services.AddSingleton<ISoketIO, SoketIO>();
            services.AddScoped<IEndMeetingReportService, EndMeetingReportService>();
            #endregion
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Use the default property (Pascal) casing
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");
            // app.ApplicationServices.GetService<IDistributedCache>;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMiddleware<ExceptionHandler>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseMiddleware<CheckEventMiddleware>();
            //app.UseMiddleware<CheckLoginClient>();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
