#nullable enable
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CMS_Lib.Extensions.Json
{
    public class JsonService
    {
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        public static string SerializeObject(Object? T)
        {
            var rs = JsonConvert.SerializeObject(T, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return rs;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        public static T DeserializeObject<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(string.IsNullOrEmpty(text) ? "" : text);
        }

        public static T DeserializeObject<T>(string text, T data)
        {
            if (string.IsNullOrEmpty(text))
            {
                return data;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            catch (Exception)
            {
                return data;
            }
        }
    }
}
