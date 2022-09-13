using System;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMS_Lib.Extensions.Attribute
{
    [AttributeUsage(AttributeTargets.All)]
    public class NoActiveMenu : System.Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Session.SetString(CmsClaimType.MenuActive, "");
        }
    }
}