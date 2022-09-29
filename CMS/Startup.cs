using System;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CMS.Extensions.Claims;
using CMS.Extensions.Notification;
using CMS.Extensions.Queue;
using CMS.Hubs;
using CMS.Middleware.AuthorizationController;
using CMS.Middleware.Hubs;
using CMS.Middleware.Menu;
using CMS.Services.Uris;
using CMS_Access.init;
using CMS_Access.Repositories;
using CMS_EF.DbContext;
using CMS_EF.Models.Identity;
using CMS_Lib;
using CMS_Lib.DI;
using CMS_Lib.Util;
using CMS_Ship.Extensions;
using CMS_WareHouse.Extensions;
using Ganss.XSS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ReflectionIT.Mvc.Paging;
using Serilog;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
using ServiceCollectionExtensions = CMS_Lib.DI.ServiceCollectionExtensions;
using UrlHelperExtensions = CMS.Extensions.Url.UrlHelperExtensions;

namespace CMS
{
    public class Startup
    {
        public static int PageSize;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSetting = Configuration.GetSection("AppSetting");
            PageSize = appSetting.GetValue<int>("PageSize");
            services.AddSingleton(Configuration);
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.CheckConsentNeeded = _ => false;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.ConsentCookie.Name = $"{appSetting.GetValue<string>("PreCookieName")}.Consent";
            });
            services.AddResponseCompression();

            services.AddResponseCaching();
            services.AddMemoryCache();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    opt => { opt.MigrationsAssembly("CMS"); });
                // options.EnableSensitiveDataLogging();
                options.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
            });
            services.AddDefaultIdentity<ApplicationUser>(o => { o.Stores.MaxLengthForKeys = 128; })
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;
                //
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Configuration.GetSection("AppSetting")
                    .GetValue<int>("DefaultLockoutTimeSpan"));
                options.Lockout.MaxFailedAccessAttempts =
                    Configuration.GetSection("AppSetting").GetValue<int>("MaxFailedAccessAttempts");
                options.Lockout.AllowedForNewUsers = true;
            });

            #region session

            services.AddAntiforgery(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = $"{appSetting.GetValue<string>("PreCookieName")}.AntiforgeryCookie";
            });

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = $"{appSetting.GetValue<string>("PreCookieName")}.Session";
            });

            services.AddCors();

            #endregion

            #region authen

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings  
                options.CookieManager = new ChunkingCookieManager();
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromDays(appSetting.GetValue<int>("ExpireTimeSpan"));
                options.LoginPath = appSetting.GetValue<string>("LoginPath");
                options.LogoutPath = appSetting.GetValue<string>("LogoutPath");
                options.AccessDeniedPath = appSetting.GetValue<string>("AccessDeniedPath");
                options.SlidingExpiration = true;
                // options.Cookie.Domain = appSetting.GetValue<string>("CookieDomain");
                options.Cookie.Name = $"{appSetting.GetValue<string>("PreCookieName")}.Cookie";
            });
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.CookieManager = new ChunkingCookieManager();
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(appSetting.GetValue<int>("ExpireTimeSpan"));
                    options.LoginPath = appSetting.GetValue<string>("LoginPath");
                    options.LogoutPath = appSetting.GetValue<string>("LogoutPath");
                    options.AccessDeniedPath = appSetting.GetValue<string>("AccessDeniedPath");
                    options.Cookie.Name = $"{appSetting.GetValue<string>("PreCookieName")}.Cookie";
                });

            #endregion

            services.AddPaging(options =>
            {
                options.ViewName = "Bootstrap5";
                options.PageParameterName = "pageindex";
                options.SortExpressionParameterName = "sort";
                options.HtmlIndicatorDown = " <span>&darr;</span>";
                options.HtmlIndicatorUp = " <span>&uarr;</span>";
            });
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext?.Request;
                var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
                return new UriService(uri);
            });
            services.AddDistributedMemoryCache();
            services.AddRouting();
            services.Configure<FormOptions>(x =>
            {
                x.BufferBody = false;
                x.KeyLengthLimit = int.MaxValue;
                x.ValueLengthLimit = int.MaxValue;
                x.ValueCountLimit = int.MaxValue;
                x.MultipartHeadersCountLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
                x.MultipartBoundaryLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
            services.AddControllersWithViews(options =>
                {
                   // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>();
                    options.EnableEndpointRouting = false;
                })
                .AddRazorRuntimeCompilation()
                .AddSessionStateTempDataProvider()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
            services.AddRazorPages()
                .AddRazorRuntimeCompilation()
                .AddSessionStateTempDataProvider()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
            services.AddHttpContextAccessor();
            AddService(services);
            AddAuthorization(services);
            services.AddSignalR();
        }

        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == Environments.Development)
            {
                // app.UseLiveReload();
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseDatabaseErrorPage();
                app.UseSerilogRequestLogging();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/Page/{0}");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            var mimeTypeProvider = new FileExtensionContentTypeProvider();
            app.UseResponseCompression();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    try
                    {
                        var headers = context.Context.Response.Headers;
                        headers[HeaderNames.CacheControl] = $"public,max-age=2592000";
                        var contentType = headers["Content-Type"];
                        if (contentType != "application/x-gzip" && !context.File.Name.EndsWith(".gz"))
                        {
                            return;
                        }

                        var fileNameToTry = context.File.Name.Substring(0, context.File.Name.Length - 3);
                        if (mimeTypeProvider.TryGetContentType(fileNameToTry, out var mimeType))
                        {
                            headers.Add("Content-Encoding", "gzip");
                            headers["Content-Type"] = mimeType;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.Request.Path.StartsWithSegments("/upload"))
                    {
                        if (ctx.Context.User.Identity is { IsAuthenticated: false })
                        {
                            ctx.Context.Response.Redirect("/");
                        }
                    }
                }
            });
            app.UseRouting();
            app.UseCors();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
                { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto }
            );
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            UrlHelperExtensions.Configure(httpContextAccessor);
            app.UseMiddleware<MenuMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<NotificationHub>("/_notification",
                    options => { options.Transports = HttpTransportType.WebSockets; });
            });
        }


        #region Add service

        public void AddService(IServiceCollection services)
        {
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<INotificationBackgroundTaskQueue, NotificationBackgroundTaskQueue>();
            services.AddHostedService<NotificationQueuedHostedService>();
            services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();
            ServiceCollectionExtensions.RegisterAllType<ITransient>(services, new[] { typeof(Program).Assembly });
            ServiceCollectionExtensions.RegisterAllType<IScoped>(services, new[] { typeof(Program).Assembly },ServiceLifetime.Scoped);
            ServiceCollectionExtensions.RegisterAllLib<ITransient>(services, typeof(LoadRepository).GetTypeInfo().Assembly);
            ServiceCollectionExtensions.RegisterAllLib<IScoped>(services, typeof(LoadRepository).GetTypeInfo().Assembly,ServiceLifetime.Scoped);
            ServiceCollectionExtensions.RegisterAllLib<ITransient>(services, typeof(AddDi).GetTypeInfo().Assembly);
            ServiceCollectionExtensions.RegisterAllLib<IScoped>(services, typeof(AddDi).GetTypeInfo().Assembly,ServiceLifetime.Scoped);
            services.AddShipCod();
            services.AddWareHouse();
        }

        #endregion

        #region Authorization

        private void AddAuthorization(IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, ControllerActionRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, NotificationHubAuthorizationHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("PermissionMVC",
                    policy => policy.Requirements.Add(new ControllerActionRequirement()));
                options.AddPolicy("NotificationHubAuthorization",
                    policy => policy.Requirements.Add(new NotificationHubAuthorizationRequirement()));
            });
        }

        #endregion

    }
}