using System.Reflection;
using ImageProxy.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace ImageProxy.Core.Services;

public interface IProxyService
{
    public ResizeParams GetResizeParams(PathString path, IQueryCollection query);
    public Task<ImageProcessResult> ImageProcess(ResizeParams resizeParams);
    public Task<ImageProcessResult> ImageNoImage(ResizeParams resizeParams);
}

public class ProxyService : IProxyService
{
    private readonly IHostingEnvironment _env;
    private readonly IMemoryCache _memoryCache;
    private static readonly string[] Suffixes = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tga" };


    public ProxyService(IHostingEnvironment env, IMemoryCache memoryCache)
    {
        _env = env;
        _memoryCache = memoryCache;
    }

    public ResizeParams GetResizeParams(PathString path, IQueryCollection query)
    {
        try
        {
            ResizeParams resizeParams = new ResizeParams();
            resizeParams.HasParams =
                resizeParams.GetType().GetTypeInfo()
                    .GetFields().Where(f => f.Name != "hasParams")
                    .Any(f => query.ContainsKey(f.Name));
            // extract resize params

            if (query.Count > 0 && query.ContainsKey("format"))
            {
                resizeParams.Format = query["format"].ToString().ToLower();
            }
            else
            {
                resizeParams.Format = path.Value.Substring(path.Value.LastIndexOf('.') + 1).ToLower();
                if (!string.IsNullOrEmpty(resizeParams.Format))
                {
                    resizeParams.Format = $".{resizeParams.Format}";
                }
            }

            resizeParams.IsImage = false;
            resizeParams.IsPhysicalFile = false;
            if (Suffixes.Any(x => x == resizeParams.Format))
            {
                resizeParams.IsImage = true;
                var imagePath = Path.Combine(_env.WebRootPath,
                    path.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));
                FileInfo fl = new FileInfo(imagePath);
                resizeParams.IsPhysicalFile = fl.Exists;
                if (fl.Exists)
                {
                    resizeParams.ImagePath = imagePath;
                }

                resizeParams.Extension = fl.Extension;
            }


            if (query.Count > 0 && query.ContainsKey("autorotate"))
            {
                bool.TryParse(query["autorotate"], out _);
            }

            int quality = 100;
            if (query.Count > 0 && query.ContainsKey("quality"))
            {
                int.TryParse(query["quality"], out quality);
            }

            resizeParams.Quality = quality;

            int w = 0;
            if (query.Count > 0 && query.ContainsKey("w"))
            {
                int.TryParse(query["w"], out w);
            }

            resizeParams.W = w;


            int h = 0;
            if (query.Count > 0 && query.ContainsKey("h"))
            {
                int.TryParse(query["h"], out h);
            }

            resizeParams.H = h;

            return resizeParams;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<ImageProcessResult> ImageProcess(ResizeParams resizeParams)
    {
        ImageProcessResult rs = new ImageProcessResult();
            long cacheKey;
            unchecked
            {
                cacheKey = resizeParams.ImagePath.GetHashCode() + resizeParams.ToString().GetHashCode();
            }

            ImageProcessResult rsCache;
            bool isCached = _memoryCache.TryGetValue(cacheKey, out rsCache);
            if (isCached)
            {
                return rsCache;
            }

            Image? image = GetImageData(resizeParams);
            if (image == null)
            {
                return null;
            }

            try
            {
                await using var ms = new MemoryStream();
                if (resizeParams.Extension.ToLower() == ".png")
                {
                    await image.SaveAsPngAsync(ms, new PngEncoder()
                    {
                        CompressionLevel = PngCompressionLevel.Level9,
                        TransparentColorMode = PngTransparentColorMode.Preserve,
                        BitDepth = PngBitDepth.Bit16,
                        //  IgnoreMetadata = true,
                        Quantizer = new WuQuantizer()
                    });
                    rs.ImageData = ms.ToArray();
                    rs.ContentType = "image/png";
                    image.Dispose();
                    _memoryCache.Set(cacheKey, rs);
                    return rs;
                }
                else if (resizeParams.Extension.ToLower() == ".jpg" || resizeParams.Extension.ToLower() == ".jpeg")
                {
                    await image.SaveAsJpegAsync(ms, new JpegEncoder() { Quality = 100 });
                    rs.ImageData = ms.ToArray();
                    rs.ContentType = "image/jpeg";
                    image.Dispose();
                    _memoryCache.Set(cacheKey, rs);
                    return rs;
                }
                else if (resizeParams.Extension.ToLower() == ".gif")
                {
                    await image.SaveAsGifAsync(ms, new GifEncoder() { ColorTableMode = GifColorTableMode.Local });
                    rs.ImageData = ms.ToArray();
                    rs.ContentType = "image/gif";
                    image.Dispose();
                    _memoryCache.Set(cacheKey, rs);
                    return rs;
                }
                else if (resizeParams.Extension.ToLower() == ".bmp")
                {
                    await image.SaveAsBmpAsync(ms,
                        new BmpEncoder() { SupportTransparency = true, BitsPerPixel = BmpBitsPerPixel.Pixel24 });
                    rs.ImageData = ms.ToArray();
                    rs.ContentType = "image/bmp";
                    image.Dispose();
                    return rs;
                }
                else if (resizeParams.Extension.ToLower() == ".tga")
                {
                    await image.SaveAsTgaAsync(ms,
                        new TgaEncoder()
                        { BitsPerPixel = TgaBitsPerPixel.Pixel24, Compression = TgaCompression.RunLength });
                    rs.ImageData = ms.ToArray();
                    rs.ContentType = "image/tga";
                    image.Dispose();
                    _memoryCache.Set(cacheKey, rs);
                    return rs;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
    }

    public async Task<ImageProcessResult> ImageNoImage(ResizeParams resizeParams)
    {
        ImageProcessResult rs = new ImageProcessResult
        {
            ContentType = "image/png"
        };
        resizeParams.ImagePath = Path.Combine(_env.WebRootPath, "images/ImageEmpty.png");
        var image = GetImageData(resizeParams);
        await using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms, new PngEncoder()
        {
            CompressionLevel = PngCompressionLevel.Level9,
            TransparentColorMode = PngTransparentColorMode.Clear,
            BitDepth = PngBitDepth.Bit16,
        });
        rs.ImageData = ms.ToArray();
        rs.ContentType = "image/png";
        image?.Dispose();
        return rs;
    }

    #region Private

    private Image? GetImageData(ResizeParams resizeParams)
    {
        Image image = Image.Load(resizeParams.ImagePath);

        if (resizeParams.H == 0)
        {
            resizeParams.H = (int)Math.Round(image.Height * (float)resizeParams.W / image.Width);
        }
        else if (resizeParams.W == 0)
        {
            resizeParams.W = (int)Math.Round(image.Width * (float)resizeParams.H / image.Height);
        }

        if (resizeParams.W == 0 && resizeParams.H == 0)
        {
            resizeParams.W = image.Width;
            resizeParams.H = image.Height;
        }

        image.Mutate(x => x.Resize(new ResizeOptions()
        {
            PremultiplyAlpha = true,
            CenterCoordinates = PointF.Empty,
            Mode = ResizeMode.Pad,
            Size = new Size(resizeParams.W, resizeParams.H),
        }));
        //image.Metadata.IptcProfile = null;
        //image.Metadata.IccProfile = null;
        return image;
    }

    #endregion
}