    using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CMS_Access.Repositories;
using CMS_Lib.Extensions.Json;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Middleware.Menu
{
    public class MenuMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public MenuMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this._next = next;
            this._configuration = configuration;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.Byte[]")]
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.User.Identity is { IsAuthenticated: true })
            {
                var claimType = _configuration.GetSection(CmsClaimType.ClaimType);
                var controllerActionType = claimType.GetValue<string>(CmsClaimType.ControllerAction);
                var menuCheck = httpContext.Session.GetString(CmsClaimType.Menu);
                if (string.IsNullOrEmpty(menuCheck))
                {
                    IClaimUserRepository iClaimUserRepository =
                        httpContext.RequestServices.GetRequiredService<IClaimUserRepository>();
                    if (iClaimUserRepository != null)
                    {
                        var userId = Int32.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                        var listControllerAction =
                            iClaimUserRepository.GetControllerActionRoleByUserClaimType(userId, controllerActionType);
                        if (listControllerAction.Count > 0)
                        {
                            var roleOld = ((ClaimsIdentity)httpContext.User.Identity)?.Claims
                                .Where(x => x.ValueType == controllerActionType).ToList();
                            if (roleOld != null)
                            {
                                foreach (var item in roleOld)
                                {
                                    ((ClaimsIdentity)httpContext.User.Identity)?.TryRemoveClaim(item);
                                }
                            }

                            listControllerAction.ForEach(x =>
                            {
                                ((ClaimsIdentity)httpContext.User.Identity)?.AddClaim(
                                    new Claim(controllerActionType, x.ToUpper()));
                            });
                        }

                        var rs = iClaimUserRepository.GetMenuByUserAndControllerAction(listControllerAction);
                        if (rs.Count > 0)
                        {
                            httpContext.Session.SetString(CmsClaimType.Menu, JsonService.SerializeObject(rs));
                        }
                    }
                }
            }

            await _next(httpContext);
        }
    }
}