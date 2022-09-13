using Microsoft.AspNetCore.Http;
using System;

namespace CMS_Lib.Extensions.Request
{
    public class RequestHelpers
    {
        public static bool IsAjaxRequest(HttpRequest request)
        {
            return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                   string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                   string.Equals(request.Headers["X-Requested-With"], "Fetch", StringComparison.Ordinal);
        }
    }
}
