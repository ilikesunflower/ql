using CMS_App_Api.Helpers.Consts;
using CMS_App_Api.Models;
using CMS_EF.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CMS_App_Api.Helpers.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : System.Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            int jWtCode = Int32.Parse(context.HttpContext.Items[ClaimTypeConst.JwtCode]?.ToString() ?? "0");
            if (jWtCode > 0)
            {
                ResultJson rs = new ResultJson
                {
                    Message = MessageCode.SecurityTokenExpired.Value,
                    Data = null,
                    StatusCode = MessageCode.SecurityTokenExpired.Code,
                    Err = ""
                };
                context.Result = new JsonResult(rs) { StatusCode = StatusCodes.Status419AuthenticationTimeout };
                return;
            }

            var user = (ApplicationUser)context.HttpContext.Items[ClaimTypeConst.User];
            if (user == null)
            {
                ResultJson rs = new ResultJson
                {
                    Message = MessageCode.Unauthorized.Value,
                    Data = null,
                    StatusCode = MessageCode.Unauthorized.Code,
                    Err = ""
                };
                context.Result = new JsonResult(rs) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }
        }
    }
}
