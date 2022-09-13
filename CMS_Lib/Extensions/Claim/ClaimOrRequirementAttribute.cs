using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Lib.Extensions.Claim
{
    public class ClaimOrRequirementAttribute : TypeFilterAttribute
    {
        public ClaimOrRequirementAttribute(string claimType, String claimValue) : base(typeof(ClaimOrRequirementFilter))
        {
            Arguments = new object[] { new System.Security.Claims.Claim(claimType, claimValue) };
        }
    }

    public class ClaimOrRequirementFilter : IAuthorizationFilter
    {
        readonly System.Security.Claims.Claim _claim;

        public ClaimOrRequirementFilter(System.Security.Claims.Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            List<string> values = _claim.Value.ToUpper().Replace(" ", "").Split(",").ToList();
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type.ToUpper() && values.Contains(c.Value));
            if (!hasClaim)
            {
                CMS_Lib.Extensions.Session.SessionExtensions.Set<string>(context.HttpContext.Session, "UrlFail", context.HttpContext.Request.Path);
                context.Result = new ForbidResult();
            }
        }
    }
}
