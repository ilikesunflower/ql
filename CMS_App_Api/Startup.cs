
using CMS_Access.init;
using CMS_App_Api.Helpers.AppSetting;
using CMS_EF.DbContext;
using CMS_EF.Models.Identity;
using CMS_Lib;
using CMS_Lib.DI;
using CMS_Ship.Extensions;
using CMS_WareHouse.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO.Compression;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace CMS_App_Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = $"api";
            });
            services.AddCors();
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.CookieManager = new ChunkingCookieManager();
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                });
            services.AddControllers(opt =>
            {
                opt.OutputFormatters.RemoveType<StringOutputFormatter>();
                opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.MaxDepth = 0;
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });
            services.AddControllersWithViews(opt =>
            {
                opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.MaxDepth = 0;
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                //options.JsonSerializerOptions.IgnoreNullValues = false;
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddHttpContextAccessor();
            AddService(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == Environments.Development)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        private void AddService(IServiceCollection services)
        {
            CMS_Lib.DI.ServiceCollectionExtensions.RegisterAllType<ITransient>(services, new[] { typeof(Program).Assembly });
            CMS_Lib.DI.ServiceCollectionExtensions.RegisterAllType<IScoped>(services, new[] { typeof(Program).Assembly }, ServiceLifetime.Scoped);
            CMS_Lib.DI.ServiceCollectionExtensions.RegisterAllLib<ITransient>(services, typeof(LoadRepository).GetTypeInfo().Assembly);
            CMS_Lib.DI.ServiceCollectionExtensions.RegisterAllLib<IScoped>(services, typeof(LoadRepository).GetTypeInfo().Assembly, ServiceLifetime.Scoped);
            CMS_Lib.DI.ServiceCollectionExtensions.RegisterAllLib<ITransient>(services, typeof(AddDi).GetTypeInfo().Assembly);
            CMS_Lib.DI.ServiceCollectionExtensions.RegisterAllLib<IScoped>(services, typeof(AddDi).GetTypeInfo().Assembly, ServiceLifetime.Scoped);
            services.AddShipCod();
            services.AddWareHouse();
        }
    }
}
