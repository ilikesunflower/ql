using System;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Extensions.Header;

public static class HeaderApplicationExtension
{
    public static IApplicationBuilder UseHeaderApplication(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException("app");
        }

        var _iConfiguration = app.ApplicationServices.GetService<IConfiguration>();
        if (_iConfiguration != null)
        {
            if (_iConfiguration.GetSection("Header:Enable").Get<bool>())
            {
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("X-Xss-Protection", _iConfiguration.GetSection("Header:X-Xss-Protection").Value);
                    context.Response.Headers.Add("X-Content-Type-Options", _iConfiguration.GetSection("Header:X-Content-Type-Options").Value);
                    context.Response.Headers.Add("Referrer-Policy", _iConfiguration.GetSection("Header:Referrer-Policy").Value);
                    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", _iConfiguration.GetSection("Header:X-Permitted-Cross-Domain-Policies").Value);
                    context.Response.Headers.Add("Content-Security-Policy", _iConfiguration.GetSection("Header:Content-Security-Policy").Value);
                    await next();
                });
            }
        }
        return app;
    }
}