using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CMS.Middleware.Hubs
{
    public class NotificationHubAuthorizationHandler : AuthorizationHandler<NotificationHubAuthorizationRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public NotificationHubAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NotificationHubAuthorizationRequirement requirement)
        {
            // Implement authorization logic  
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.Identity is { IsAuthenticated: true })
            {
                // Authorization passed  
                context.Succeed(requirement);
            }
            else
            {
                // Authorization failed  
                context.Fail();
            }

            // Return completed task  
            return Task.CompletedTask;
        }
    }
}
