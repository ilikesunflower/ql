using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Lib.Extensions.Claim
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new System.Security.Claims.Claim(claimType, claimValue) };
        }
    }
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly System.Security.Claims.Claim _claim;

        public ClaimRequirementFilter(System.Security.Claims.Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            List<string> values = _claim.Value.ToUpper().Replace(" ", "").Split(",").ToList();
            foreach (var v in values)
            {
                var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type.ToUpper() && c.Value == v.Trim());
                if (!hasClaim)
                {
                    CMS_Lib.Extensions.Session.SessionExtensions.Set<string>(context.HttpContext.Session, "UrlFail", context.HttpContext.Request.Path);
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
