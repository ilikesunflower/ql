using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMS.Extensions.Validate;

[AttributeUsage(AttributeTargets.All)]
public class ValidHeaderAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        base.OnResultExecuting(context);
        var cookies = context.HttpContext.Request.Cookies.Select(x => x.Value).ToList();
        if (cookies.Count > 0)
        {
            foreach (var item in cookies)
            {
                bool isRedirect = ValidCookies(item);
                if (isRedirect)
                {
                    context.Result = new NoContentResult();
                    return;
                }
            }
        }
    }

    private static List<string> _tagCookies = new List<string>() { "and", "or", "ping", "limit", "copy", "select", "to",
        "jndi:", "ldap:","cmd=","&","exec" };
    private bool ValidCookies(string v)
    {
        try
        {
            if (string.IsNullOrEmpty(v))
            {
                return false;
            }

            foreach (var item in _tagCookies)
            {
                if (v.ToLower().Contains(item))
                {
                    return true;
                }
            }
            
        }
        catch (Exception)
        {
            // ignored
        }

        return false;
    }
}