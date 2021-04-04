using AutoMapper;
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
using Project.App.DesignPatterns.Repositories;
using Project.App.Mappings;
using Project.App.Middlewares;
using Project.App.Schedules;
using Project.App.Schedules.Jobs;
using Project.Modules.Accounts.Services;
using Project.Modules.Classes.Services;
using Project.Modules.ClassSchedules.Services;
using Project.Modules.Contacts.Services;
using Project.Modules.Courses.Services;
using Project.Modules.CourseSubjects.Services;
using Project.Modules.Groups.Sevices;
using Project.Modules.Holidays.Services;
using Project.Modules.Lecturers.Services;
using Project.Modules.MoneyTypes.Services;
//using Project.Modules.OpenVidus.Services;
using Project.Modules.Receipts.Services;
using Project.Modules.Reports.Services;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.SchoolYears.Services;
using Project.Modules.Scores.Services;
using Project.Modules.ScoreTypes.Services;
using Project.Modules.Semesters.Services;
using Project.Modules.Slots.Services;
using Project.Modules.Students.Services;
using Project.Modules.SubjectGroups.Services;
using Project.Modules.Subjects.Services;
using Project.Modules.Users.Services;
using Quartz;
using System.Text;

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

            #region Transport Pattern Provider
            ObserverPatternHandling.Instance.Connection(Configuration);
            #endregion

            #region Add Cron Job Service
            services.UseQuartz(typeof(SampleJob));
            #endregion

            #region Add Context Service
            services.AddDbContextPool<MariaDBContext>(options => {
                options.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
                });
            services.AddSingleton<MongoDBContext>();
            services.AddSingleton<RedisDBContext>();

            // var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            #endregion

            #region Add Module Services

            services.AddScoped<IRepositoryMariaWrapper, RepositoryMariaDBWrapper>();
            services.AddScoped<IRepositoryMongoWrapper, RepositoryMongoWrapper>();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IStudentService, StudentService>();  
            //services.AddScoped<IViduService, ViduService>();  
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ILecturerService, LecturerService>();
            services.AddScoped<IClassScheduleService, ClassScheduleService>();
            
            services.AddScoped<ISlotService, SlotService>();
            services.AddScoped<ISubjectGroupService, SubjectGroupService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ISchoolYearService, SchoolYearService>();
            services.AddScoped<IScoreTypeService, ScoreTypeService>();
            services.AddScoped<ISocreTypeSubjectService, SocreTypeSubjectService>();
            services.AddScoped<ICourseService, CourseService>();

            services.AddScoped<IReportScoreService, ReportScoreService>();
            services.AddScoped<IScoreService, ScoreService>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<ICourseSubjectService, CourseSubjectService>();
            services.AddScoped<IHolidayServices, HolidayServices>();
            services.AddScoped<ISupportScoreService, SupportScoreService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IHelperScoreService, HelperScoreService>();
            services.AddScoped<IHelperCourseSubjectService, HelperCourseSubjectService>();
            services.AddScoped<IReceiptDetailService, ReceiptDetailService>();
            services.AddScoped<IReportReceiptService, ReportReceiptService>();
            services.AddScoped<IMoneyTypeService, MoneyTypeService>();
            services.AddScoped<ILectureSalaryService, LectureSalaryService>();
            services.AddScoped<IEmployeeSalaryService, EmployeeSalaryService>();

            #endregion

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

            services.AddDistributedMemoryCache();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                // Use the default property (Pascal) casing
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.SetUtilsProviderConfiguration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            #region Start Job
            IScheduler scheduler = app.ApplicationServices.GetService<IScheduler>();
            // QuartzServicesUtilitie.StartJob<SampleJob>(scheduler, Configuration["SystemAction:SummarizeRatingJob"]);
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
