using ImageProxy.Core.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace ImageProxy.Extensions;

public static class ImageProxyBuilder
{
    public static IApplicationBuilder UseProxyImageApplication(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.UseMiddleware<ImageProxyMiddleware>();
        return app;
    }
}