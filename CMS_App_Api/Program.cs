using CMS_App_Api;
using Microsoft.AspNetCore.Builder;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext());
Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up!");
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
#pragma warning disable CS0612 // Type or member is obsolete
startup.Configure(app, app.Environment);
#pragma warning restore CS0612 // Type or member is obsolete

app.Run();
