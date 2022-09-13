
using CMS_Lib.Extensions.Json;
using Microsoft.AspNetCore.Http;

namespace CMS_Lib.Extensions.Session
{
    public class SessionExtensions
    {
        public static void Set<T>(HttpContext httpContext, string key, T value)
        {
            httpContext.Session.SetString(key, JsonService.SerializeObject(value));
        }
        
        public static T Get<T>(HttpContext httpContext, string key)
        {
            var value = httpContext.Session.GetString(key);
            return value == null ? default(T) : JsonService.DeserializeObject<T>(value);
        }
        
        public static void Set<T>(ISession session, string key, T value)
        {
            session.SetString(key, JsonService.SerializeObject(value));
        }

        public static T Get<T>(ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonService.DeserializeObject<T>(value);
        }
    }
}
