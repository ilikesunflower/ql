using CMS_Access.Repositories;
using CMS_Lib.DI;
using CMS_Lib.Extensions.StaticFiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using CMS_Lib.Util;
using Xabe.FFmpeg;

namespace CMS.Services.Files
{
    public interface IFileService : IScoped
    {
        Task<CMS_EF.Models.Files> UploadFile(IFormFile ufile, int userId);

        string UploadAvatar(IFormFile ufile, int userId);
        string SavingFile(IFormFile? ufile, string storePath= "upload", string fileName = "");
        string SavingFileP(IFormFile? ufile, string storePath= "upload/P", string fileName = "");

        bool CheckSizeFile(long bytes, int maxSize);

        Task<string> UploadFileImport(IFormFile uFile, string folder);

        string RandomFileName(string directory, string fileName, int numberFile = 0);
    }
    [Obsolete]
    public class FileService : IFileService
    {
        private readonly IFilesRepository iFilesRepository;

        private readonly ILogger<FileService> _logger;

        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly IConfigurationSection appSettings;

        private readonly int imageSize = 150;
        private readonly int imageQuality = 75;
        private const string _defaultTypeVideo = ".mp4";
        private readonly string _defaultUploadImage;
        private readonly string _defaultUploadVideo;
        private readonly string _defaultUploadOther;

        [Obsolete]
        public FileService(IFilesRepository iFilesRepository, IWebHostEnvironment env, ILogger<FileService> logger, IConfiguration configuration)
        {
            this.iFilesRepository = iFilesRepository;
            this._hostingEnvironment = env;
            this._logger = logger;
            this.appSettings = configuration.GetSection("AppSetting");
            this.imageSize = this.appSettings.GetValue<int>("ImageMaxSize");
            this.imageQuality = this.appSettings.GetValue<int>("ImageQuality");
            this._defaultUploadImage = this.appSettings.GetValue<string>("UploadImages");
            this._defaultUploadVideo = this.appSettings.GetValue<string>("UploadVideos");
            this._defaultUploadOther = this.appSettings.GetValue<string>("UploadOthers");
        }

        public async Task<CMS_EF.Models.Files> UploadFile(IFormFile ufile, int userId)
        {
            if (ufile != null && ufile.Length > 0)
            {
                string contentType = ufile.ContentType;
                DateTime t = DateTime.Now;
                CMS_EF.Models.Files files = new CMS_EF.Models.Files
                {
                    Type = FileExtensionContentTypeProvider.GetFileType(contentType)
                };
                bool isSaveFilePhysical = false;
                if (ufile.ContentType.StartsWith("image"))
                {
                    string dicrectoryName =
                        $"{this._defaultUploadImage}/{t.Year}/{t.Month:D2}/{t.Day:D2}";
                    var directory = Path.Combine(_hostingEnvironment.WebRootPath, dicrectoryName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var listTypeFile = Path.GetFileName(ufile.FileName).Split(".");
                    string name = ufile.FileName.Substring(0, ufile.FileName.LastIndexOf(listTypeFile[^1], StringComparison.Ordinal) - 1);
                    string guid = Guid.NewGuid().ToString();
                    var fileName = CmsFunction.RewriteUrlUpload($"{name}-{guid}.{listTypeFile[^1]}");
                    await using (var fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
                    {
                        await ufile.CopyToAsync(fileStream);
                        fileStream.Flush();
                    }
                    files.Url = $"/{dicrectoryName}/{fileName}";
                    files.Name = ufile.FileName;
                    files.Thumbnail = FileExtensionContentTypeProvider.GetThumbnailHtml(files.Type, files.Url);
                    isSaveFilePhysical = true;
                }
                else if (ufile.ContentType.StartsWith("video"))
                {
                    string dicrectoryName =
                        $"{this._defaultUploadVideo}/{t.Year}/{t.Month:D2}/{t.Day:D2}";
                    var directory = Path.Combine(_hostingEnvironment.WebRootPath, dicrectoryName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var listTypeFile = ufile.FileName.Split(".");
                    string name = ufile.FileName.Substring(0, ufile.FileName.LastIndexOf(listTypeFile[^1], StringComparison.Ordinal) - 1);
                    string guid = Guid.NewGuid().ToString();
                    string fileName = $"{name}{guid}{_defaultTypeVideo}";
                    string url = $"/{dicrectoryName}/{fileName}";
                    string pathAbsolute = Path.Combine(directory, $"{fileName}");
                    string rs = await SavePhysicalVideo(pathAbsolute, ufile);
                    if (string.IsNullOrEmpty(rs))
                    {
                        return null;
                    }

                    FileInfo info = new FileInfo(pathAbsolute);
                    string thumbnail = $"/{dicrectoryName}/{guid}.png";
                    var thumbnailAbsolute = await GenerateThumbnailVideo(Path.Combine(directory, $"{guid}.png"), info);
                    if (thumbnailAbsolute != null)
                    {
                        files.Thumbnail = thumbnail;
                    }
                    files.Name = $"{name}{_defaultTypeVideo}";
                    files.Url = url;
                    isSaveFilePhysical = true;
                }
                else
                {
                    string dicrectoryName =
                        $"{this._defaultUploadOther}/{t.Year}/{t.Month:D2}/{t.Day:D2}";
                    var directory = Path.Combine(_hostingEnvironment.WebRootPath, dicrectoryName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var listTypeFile = Path.GetFileName(ufile.FileName).Split(".");
                    string name = ufile.FileName.Substring(0, ufile.FileName.LastIndexOf(listTypeFile[^1], StringComparison.Ordinal) - 1);
                    string guid = Guid.NewGuid().ToString();
                    var fileName = CmsFunction.RewriteUrlUpload($"{name}-{guid}.{listTypeFile[^1]}");
                    await using (var fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
                    {
                        await ufile.CopyToAsync(fileStream);
                        fileStream.Flush();
                    }
                    files.Url = $"/{dicrectoryName}/{fileName}";
                    files.Name = ufile.FileName;
                    files.Thumbnail = FileExtensionContentTypeProvider.GetThumbnailHtml(files.Type, files.Url);
                    isSaveFilePhysical = true;
                }
                if (isSaveFilePhysical)
                {
                    files.ContentType = contentType;
                    files.CreatedBy = userId;
                    files.Name = string.IsNullOrEmpty(files.Name) ? files.Name : files.Name.Replace("'", "").Replace(" ", "_");
                    files.Size = SizeConverter(ufile.Length);
                    files.CreatedAt = DateTime.Now;
                    iFilesRepository.Create(files);
                    return files;
                }
            }
            return null;
        }

        [Obsolete]
        public string UploadAvatar(IFormFile ufile, int userId)
        {
            try
            {
                if (ufile != null)
                {
                    DateTime t = DateTime.Now;
                    string dicrectoryName = $"upload/avatar/{userId}/{t.Year}/{t.Month:D2}/{t.Day:D2}";
                    var directory = Path.Combine(_hostingEnvironment.WebRootPath, dicrectoryName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var fileName = CmsFunction.RewriteUrlUpload(Path.GetFileName(ufile.FileName));
                    using var fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
                    ufile.CopyTo(fileStream);
                    fileStream.Flush();

                    return $"/{dicrectoryName}/{fileName}";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                return null;
            }
        }

        [Obsolete]
        public string SavingFile(IFormFile ufile, string storePath = "upload", string fileName = "")
        {
            try
            {
                if (ufile != null)
                {
                    DateTime t = DateTime.Now;
                    string dicrectoryName = $"{storePath}/{t.Year}/{t.Month:D2}/{t.Day:D2}";
                    var directory = Path.Combine(_hostingEnvironment.WebRootPath, dicrectoryName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (fileName == "")
                    {
                         fileName = RandomFileName(directory, ufile.FileName.ToLower());
                         fileName = CmsFunction.RewriteUrlUpload(fileName);
                    }
                    using var fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
                    ufile.CopyTo(fileStream);
                    fileStream.Flush();
                    return $"/{dicrectoryName}/{fileName}";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                return null;
            }
        }      [Obsolete]
        public string SavingFileP(IFormFile ufile, string storePath = "upload/p", string fileName = "")
        {
            try
            {
                if (ufile != null)
                {
                    DateTime t = DateTime.Now;
                    string dicrectoryName = $"{storePath}/{t.Year}/{t.Month:D2}/{t.Day:D2}";
                    var directory = Path.Combine(_hostingEnvironment.WebRootPath, dicrectoryName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (fileName == "")
                    {
                         fileName = RandomFileName(directory, ufile.FileName.ToLower());
                         fileName = CmsFunction.RewriteUrlUpload(fileName);
                    }
                    using var fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
                    ufile.CopyTo(fileStream);
                    fileStream.Flush();
                    return $"/{dicrectoryName}/{fileName}";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                return null;
            }
        }
        
        public bool CheckSizeFile(long bytes, int maxSize)
        {
            var megabyte = new decimal(1024 * 1024);
            var mbSize = maxSize * megabyte;
            if (bytes > mbSize)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<string> UploadFileImport(IFormFile uFile, string folder)
        {
            if (uFile is {Length: > 0})
            {
                DateTime t = DateTime.Now;
                string directoryName = $"upload/insertData/{folder}/{t.Year}/fileRoot";
                var directory = Path.Combine(_hostingEnvironment.WebRootPath, directoryName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string filename = CmsFunction.RewriteUrlUpload(RandomFileName(directory, uFile.FileName.ToLower()));
                await using var fileStream = new FileStream(Path.Combine(directory, filename), FileMode.Create);
                await uFile.CopyToAsync(fileStream);
                fileStream.Flush();
                return $"{directoryName}/{filename}";
            }
            return null;
        }

        public string RandomFileName(string directory, string fileName, int numberFile = 0)
        {
            string name = fileName;
            if (numberFile != 0)
            {
                name = $"{numberFile}_{fileName}";
            }
           
            if (File.Exists(Path.Combine(directory, name)))
            {
                name = RandomFileName(directory, fileName, numberFile + 1);
            }
            return name;
        }

        public static string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            return fileSize switch
            {
                _ when fileSize < kilobyte => $"Less then 1KB",
                _ when fileSize < megabyte =>
                    $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB",
                _ when fileSize < gigabyte =>
                    $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB",
                _ when fileSize >= gigabyte =>
                    $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB",
                _ => "n/a"
            };
        }
        
        [Obsolete]
        private string SavePhysicalImage(String url, IFormFile ufile)
        {
            if (ufile != null)
            {
                try
                {
                    //using var inputStream = new SKManagedStream(ufile.OpenReadStream());
                    //using var original = SKBitmap.Decode(inputStream);
                    //int width, height;
                    //if (original.Width > original.Height)
                    //{
                    //    width = this.imageSize;
                    //    height = original.Height * this.imageSize / original.Width;
                    //}
                    //else if (original.Width < this.imageSize && original.Height < this.imageSize)
                    //{
                    //    width = original.Width;
                    //    height = original.Height;
                    //}
                    //else
                    //{
                    //    width = original.Width * this.imageSize / original.Height;
                    //    height = this.imageSize;
                    //}

                    //using var resized = original.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3);
                    //if (resized == null) return null;
                    //using var image = SKImage.FromBitmap(resized);
                    //using var output = File.OpenWrite(url);
                    //image.Encode(SKEncodedImageFormat.Jpeg, this.imageQuality).SaveTo(output);

                    using var fileStream = new FileStream(Path.Combine(url), FileMode.Create);
                    ufile.CopyTo(fileStream);
                    fileStream.Flush();

                    return url;
                }
                catch (Exception ex)
                {
                    this._logger.LogError("Lỗi upload file", ex);
                    return null;
                }
            }
            return null;
        }

        private async Task<string> SavePhysicalVideo(string url, IFormFile ufile)
        {
            if (ufile != null)
            {
                try
                {
                    await using var fileStream = new FileStream(url, FileMode.Create);
                    await ufile.CopyToAsync(fileStream);
                    fileStream.Flush();
                    return url;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        private async Task<string> GenerateThumbnailVideo(string url, FileInfo ufile)
        {
            try
            {
                //string ffmpregLink = this.appSettings.GetValue<string>("FFmpegLink");
                //if (!string.IsNullOrEmpty(ffmpregLink))
                //{
                //    FFmpeg.SetExecutablesPath(ffmpregLink);
                //}
                var data = FFmpeg.Conversions.FromSnippet.Snapshot(ufile.FullName, url, TimeSpan.FromSeconds(0)).Result;
                IConversionResult result = await data.Start();
                this._logger.LogInformation($"GenerateThumbnailVideo: {ufile.FullName} : {result.Arguments} : {result}");
                return url;
            }
            catch (Exception ex)
            {
                this._logger.LogError($"GenerateThumbnailVideo {ufile.FullName} {ex.Message} {ex.ToString()} {ex.StackTrace}", ex.Message);
                return null;
            }
        }
        
    }
}
