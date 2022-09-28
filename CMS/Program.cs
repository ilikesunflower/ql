using System;
using System.IO;
using System.Reflection;
using CMS;
using CMS.Config.Consts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext());
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    options.Limits.MaxRequestBodySize = null;
});
builder.WebHost.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
builder.WebHost.UseDefaultServiceProvider(o =>
{
    o.ValidateOnBuild = false;
    o.ValidateScopes = false;
});

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up!");
AppConst.Domain = $"{builder.Configuration.GetSection("AppSetting:Domain").Value}";
AppConst.DomainFE = $"{builder.Configuration.GetSection("AppSetting:FEDomain").Value}";
#region FFmpeg
try
{
    Log.Information("[Start] created FFmpeg");
    string path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
    FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, path).GetAwaiter().GetResult();
    FFmpeg.SetExecutablesPath(path);
    Log.Information("[End] created FFmpeg");
}
catch (Exception)
{
    // ignored
}
#endregion

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
#pragma warning disable CS0612 // Type or member is obsolete
startup.Configure(app, app.Environment);
#pragma warning restore CS0612 // Type or member is obsolete

app.Run();
Log.CloseAndFlush();