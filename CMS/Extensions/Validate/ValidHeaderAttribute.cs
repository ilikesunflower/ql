using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace CMS.Extensions.Validate;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ValidHeaderAttribute : TypeFilterAttribute
{
    public ValidHeaderAttribute() : base(typeof(AllowCookiesFilter))
    {
    }
}

public class AllowCookiesFilter : IAuthorizationFilter
{
    private List<string> _tagNotAllow = new List<string>()
    {
        "and ", "or ", "ping ", "limit ", "COPY", "select ", "to ",
        "jndi:", "ldap:", "cmd=", "&", "exec", "primefaces", "ping -c"
    };

    public AllowCookiesFilter()
    {
    }

    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            KeyValuePair<string, StringValues>? cookies =
                context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Cookie");
            if (cookies != null)
            {
                var c = cookies.Value.Value;
                if (c.Count > 0)
                {
                    foreach (var item in c)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            foreach (var i in _tagNotAllow)
                            {
                                if (item.ToLower().Contains(i))
                                {
                                    context.Result = new BadRequestResult();
                                    return;
                                }
                            }
                        }
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