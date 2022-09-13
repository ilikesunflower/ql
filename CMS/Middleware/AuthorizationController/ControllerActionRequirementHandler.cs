using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CMS.Middleware.AuthorizationController
{
    public class ControllerActionRequirementHandler : AuthorizationHandler<ControllerActionRequirement>
    {
        private readonly IConfiguration configuration;
        private readonly IConfigurationSection webSetting;
        private readonly IHttpContextAccessor httpContextAccessor;


        public ControllerActionRequirementHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.webSetting = configuration.GetSection("WebSetting");
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ControllerActionRequirement requirement)
        {

            var claimType = configuration.GetSection("ClaimType");
            if (true)
            {
                var ctx = httpContextAccessor;
                if (ctx.HttpContext?.User.Identity != null && ctx.HttpContext.User.Identity.IsAuthenticated)
                {
                    var intention = ctx.HttpContext.Request.Method;
                    var routeData = ctx.HttpContext.GetRouteData();
                    {
                        var controller = $"{routeData.Values["area"]?.ToString()}@{(routeData.Values["controller"]?.ToString() + "Controller")}".ToUpper();
                        var action = routeData.Values["action"]?.ToString()?.ToUpper();
                        if (ctx.HttpContext.User.HasClaim(claimType["ControllerAction"], controller + "@" + action) || ctx.HttpContext.User.HasClaim(claimType["Controller"], controller) || webSetting.GetValue<int>("AllPermission") == 1)
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            if (httpContextAccessor.HttpContext != null)
                                CMS_Lib.Extensions.Session.SessionExtensions.Set<string>(httpContextAccessor.HttpContext.Session, "UrlFail", ctx.HttpContext.Request.Path);
                        }
                    }
                }
            }
            await Task.Yield();
        }
    }
}
