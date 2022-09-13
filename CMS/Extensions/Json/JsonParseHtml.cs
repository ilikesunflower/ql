using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Extensions.Json
{
  
    public static class JsonParseHtml
    {
       
        public static string JsonParse( Object ob)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            if (ob == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(ob, jsonSerializerSettings);
        }

    }
}
