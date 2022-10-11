using ImageProxy.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace ImageProxy.Core.Middlewares;

public class ImageProxyMiddleware
{
    private readonly RequestDelegate _next;


    public ImageProxyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (context.Request.Method != HttpMethods.Get)
            {
                await _next.Invoke(context);
                return;
            }
            var path = context.Request.Path;
            if (path.HasValue && path.Value.ToLower().StartsWith("/images"))
            {
                await _next.Invoke(context);
                return;
            }
        
            IProxyService? imageService = context.RequestServices.GetService<IProxyService>();
            var resizeParams = imageService!.GetResizeParams(path, context.Request.Query);
            if (resizeParams is { IsImage: true })
            {
                var headers = context.Response.Headers;
                headers[HeaderNames.CacheControl] = $"public,max-age=2592000";
                if (resizeParams.IsPhysicalFile)
                {
                    var rs = await imageService.ImageProcess(resizeParams);
                    if (rs.ImageData is { Length: > 0 })
                    {
                        context.Response.ContentType = rs.ContentType;
                        context.Response.ContentLength = rs.ImageData.Length;
                        await context.Response.Body.WriteAsync(rs.ImageData, 0, rs.ImageData.Length);
                        return;
                    }
                }
        
                var rsNoImage = await imageService.ImageNoImage(resizeParams);
                if (rsNoImage.ImageData is { Length: > 0 })
                {
                    context.Response.ContentType = rsNoImage.ContentType;
                    context.Response.ContentLength = rsNoImage.ImageData.Length;
                    await context.Response.Body.WriteAsync(rsNoImage.ImageData, 0, rsNoImage.ImageData.Length);
                    return;
                }
        
                await _next.Invoke(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
        catch
        {
            // ignored
        }
    }
}

