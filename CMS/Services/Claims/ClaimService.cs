using CMS_Lib.DI;
using System;
using System.Security.Claims;
using CMS_EF.Models.Identity;
using CMS_Lib.Util;
using Microsoft.Extensions.Logging;

namespace CMS.Services.Claims
{
    public interface IClaimService : IScoped
    {
        bool RemoveClaimByUser(ClaimsPrincipal user, string key);

        bool ReloadClaimByUser(ClaimsPrincipal user, string key, string value);

        bool ReloadInfoUser(ClaimsPrincipal user, ApplicationUser userInfo);
    }

    public class ClaimService : IClaimService
    {
        private readonly ILogger<ClaimService> _iLogger;

        public ClaimService(ILogger<ClaimService> iLogger)
        {
            this._iLogger = iLogger;
        }

        public bool RemoveClaimByUser(ClaimsPrincipal user, string key)
        {
            try
            {
                var identity = user.Identity as ClaimsIdentity;
                var claim = identity?.FindFirst(key);
                if (claim != null)
                {
                    identity?.TryRemoveClaim(claim);
                }
                return true;
            }
            catch (Exception ex)
            {
                this._iLogger.LogError($"RemoveClaimByUser {key}", ex);
                return false;
            }
        }

        public bool ReloadClaimByUser(ClaimsPrincipal user, string key, string value)
        {
            try
            {
                var identity = user.Identity as ClaimsIdentity;
                var claim = identity?.FindFirst(key);
                if (claim != null)
                {
                    identity?.TryRemoveClaim(claim);
                }
                identity?.AddClaim(new Claim(key, value));
                return true;
            }
            catch (Exception ex)
            {
                this._iLogger.LogError($"ReloadClaimByUser {key} - {value}", ex);
                return false;
            }
        }

        public bool ReloadInfoUser(ClaimsPrincipal user, ApplicationUser userInfo)
        {
            ReloadClaimByUser(user, ClaimTypes.Name, userInfo.FullName ?? "");
            ReloadClaimByUser(user, CmsClaimType.UserName, userInfo.UserName ?? "");
            ReloadClaimByUser(user, ClaimTypes.NameIdentifier, userInfo.Id.ToString() ?? "");
            ReloadClaimByUser(user, CmsClaimType.Avatar, userInfo.Image ?? "");
            ReloadClaimByUser(user, CmsClaimType.UserType, userInfo.Type + "");
            ReloadClaimByUser(user, CmsClaimType.IsActiveUser, userInfo.IsActive + "");
            return true;
        }
    }
}
