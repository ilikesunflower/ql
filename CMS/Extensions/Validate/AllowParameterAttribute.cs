using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMS.Extensions.Validate;



[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AllowParameterAttribute : TypeFilterAttribute
{
    public AllowParameterAttribute() :  base(typeof(AllowParameterFilter))
    {
    }
}

public class AllowParameterFilter : IAuthorizationFilter
{
    private List<string> _tagNotAllow = new List<string>() { "isadmin","issso","role","root" };

    public AllowParameterFilter()
    {
        
    }
    
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            var req = context.HttpContext.Request;
            req.EnableBuffering();
            req.Body.Position = 0;
            var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
            req.Body.Position = 0;
            if (!string.IsNullOrEmpty(body))
            {
                foreach (var item in _tagNotAllow)
                {
                    if (body.ToLower().Contains(item))
                    {
                        context.Result = new BadRequestResult();
                        return;
                    }
                }
            }
        }
        catch
        {
            //
        }
    }

}

