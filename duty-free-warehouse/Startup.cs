using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project.App.Database;
using Project.App.Middleware;
using Project.App.Providers;
using Project.Modules.Products.Services;
using Project.Modules.Declarations.Services;
using Project.Modules.Users.Services;
using System.Text;
using Project.Modules.Seals.Services;
using Project.Modules.Inventories.Services;
using Project.Modules.FileUploads.Service;
using Project.Modules.Destroys.Services;
using Project.Modules.SellSeals;
using Project.Modules.SettlementReports.Services;
using Project.Modules.Exchangerates.Services;
using Project.Modules.Declarations.Models;

namespace Project
{
    public class Startup
    {
        public IConfiguration _config { get; }
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.SetUtilsProviderConfiguration(_config);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"].ToString(),
                    ValidAudience = _config["Jwt:Issuer"].ToString(),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"].ToString()))
                };
            });

            services.AddDbContextPool<MariaDBContext>(options => options.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]));
            services.AddSingleton<MongoDBContext>();
            services.AddSingleton<RedisDBContext>();
            #region Module Services
            services.AddScoped<IDeClarationServices, DeClarationServices>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IServiceUser,ServiceUser>();
            services.AddScoped<IImportExportCartService, ImportExportCartService>();
            services.AddScoped<ISealImportService, SealImportService>();
            services.AddScoped<IDeClarationDetailServices, DeClarationDetailServices>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IDestroyService, DestroyService>();
            services.AddScoped<IDestroyDetailService, DestroyDetailService>();
            services.AddScoped<ISellSealSerivce, SellSealSerivce>();
            services.AddScoped<IReportExportService, ReportExportService>();
            services.AddScoped<ISettlementReportService, SettlementReportService>();
            services.AddScoped<ICityPairService, CityPairService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IServiceDepartmentPermission, ServiceDepartmentPermission>();
            services.AddScoped<IServicePermission, ServicePermission>();
            services.AddScoped<IServiceUserPermission, ServiceUserPermission>();
            services.AddScoped<IExchangerateService, ExchangerateService>();
            #endregion
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc()
            .AddJsonOptions(
            options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddDistributedMemoryCache();
            TransportPatternHandlingProvider.Instance.Connection(_config);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseMiddleware<ExceptionHandler>();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
