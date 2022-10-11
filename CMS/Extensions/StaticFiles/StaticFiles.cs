using System;
using AngleSharp.Io;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

namespace CMS.Extensions.StaticFiles;

public static class StaticFiles
{
    public static IApplicationBuilder UseStaticFilesApplication(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        var mimeTypeProvider = new FileExtensionContentTypeProvider();
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
        return app;
    }
}