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
    public class AuthorizeRefreshTokenAttribute : System.Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (ApplicationUser)context.HttpContext.Items[ClaimTypeConst.User];
            string tokenType = context.HttpContext.Items[ClaimTypeConst.TokenType]?.ToString();
            string key = "refreshToken";
            if (user == null || (tokenType != key))
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
