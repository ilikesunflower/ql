using System;
using System.Collections.Generic;

namespace CMS_Lib.Extensions.StaticFiles
{
    public class FileExtensionContentTypeProvider
    {


        public static Dictionary<string, int> DictionaryType = new Dictionary<string, int>()
        {
            // img
                { "image/x-jg",1 },
                { "image/bmp", 1 },
                { "image/x-cmx",1 },
                { "image/cis-cod",1 },
                { "image/gif",1 },
                { "image/x-icon",1 },
                { "image/ief",1 },
                { "image/pjpeg",1 },
                { "image/jpeg",1 },
                { "image/x-portable-graymap",1 },
                { "image/png",1 },
                { "image/x-portable-anymap",1 },
                { "image/x-cmu-raster",1 },
                { "image/vnd.rn-realflash",1 },
                { "image/x-rgb",1 },
                { "image/svg+xml",1 },
                // video
                { "video/3gpp2",2 },
                { "video/3gpp",2 },
                { "video/x-ms-asf",2 },
                { "video/x-msvideo",2 },
                { "video/x-ms-dvr",2 },
                { "video/x-flv",2 },
                { "video/x-ivf",2 },
                { "video/x-la-asf",2 },
                { "video/vnd.dlna.mpeg-tts",2 },
                { "video/quicktime",2 },
                { "video/x-sgi-movie",2 },
                { "video/mp4",2 },
                { "video/ogg",2 },
                {"video/mpeg",2 },
                // audio
                {"audio/vnd.dlna.adts",3 },
                {"audio/x-aiff",3 },
                {"audio/aiff",3 },
                {"audio/basic",3 },
                {"audio/mid",3 },
                {"audio/mpeg",3 },
                {"audio/ogg",3 },
                {"audio/mp4",3 },
                {"audio/mp3",3 },
                // pdf
                {"application/pdf",4 },
                // word
                {"application/msword",5 },
                {"application/vnd.ms-word.document.macroEnabled.12",5 },
                {"application/vnd.openxmlformats-officedocument.wordprocessingml.document",5 },
                {"application/vnd.ms-word.template.macroEnabled.12",5 },
                {"application/vnd.openxmlformats-officedocument.wordprocessingml.template",5 },
                // excel
                {"application/vnd.ms-excel",6 },
                {"application/vnd.ms-excel.sheet.binary.macroEnabled.12",6 },
                {"application/vnd.ms-excel.sheet.macroEnabled.12",6 },
                {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",6 },
                {"application/vnd.ms-excel.template.macroEnabled.12",6},
                {"application/vnd.openxmlformats-officedocument.spreadsheetml.template",6 },
                // pptx
                {"application/vnd.ms-powerpoint",7 },
                {"application/vnd.ms-powerpoint.presentation.macroEnabled.12",7 },
                {"application/vnd.openxmlformats-officedocument.presentationml.presentation",7 },
                // nén
                { "application/zip", 8 },
                { "application/rar", 8 },
                { ".rar", 8 },
                { "application/octet-stream", 8 },
                { "application/x-gzip", 8 },
                { "application/x-zip-compressed", 8 },
                { "application/x-compress", 8 }
        };

        public static int GetFileType(string contentType)
        {
            try
            {
                if (DictionaryType.TryGetValue(contentType, out var value))
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static string GetThumbnailHtml(int type, string url)
        {
            try
            {
                string html = "";
                switch (type)
                {
                    case 1:
                        html = url;
                        break;
                    case 2:
                        html = url;
                        break;
                    case 3:
                        html = "/images/icon/mp3.png";
                        break;
                    case 4:
                        html = "/images/icon/pdf.png";
                        break;
                    case 5:
                        html = "/images/icon/doc.png";
                        break;
                    case 6:
                        html = "/images/icon/excel.png";
                        break;
                    case 7:
                        html = "/images/icon/powerpoint.png";
                        break;
                    case 8:
                        html = "/images/icon/zip.png";
                        break;
                    case 0:
                        html = "/images/icon/file.png";
                        break;
                }
                return html;
            }
            catch
            {
                return null;
            }
        }

        public static string GetAllowTypeFile()
        {
            string rs = "image/*,video/*,audio/*,application/pdf,.doc,.docx,.csv,.xls,.xlsx,.zip,.rar,application/x-gzip,application/x-zip-compressed";
            return rs;
        }

        public static bool CheckAllowTypeFile(string contentType)
        {
            Boolean rs = DictionaryType.ContainsKey(contentType);
            return rs;
        }
    }
}
