using System.Diagnostics.CodeAnalysis;
using CMS.Services.Claims;
using CMS_Access.Repositories;
using CMS_EF.Models.Identity;
using CMS_Lib.Extensions.Json;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Extensions.Claims
{
    public sealed class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        private readonly IClaimUserRepository _iClaimUserRepository;
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private readonly IClaimService _iClaimService;

        public AppClaimsPrincipalFactory(IClaimUserRepository iClaimUserRepository, IConfiguration configuration, IClaimService iClaimService,
            UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor, IHttpContextAccessor iHttpContextAccessor) : base(userManager,
            optionsAccessor)
        {
            this._iClaimUserRepository = iClaimUserRepository;
            _configuration = configuration;
            this._iHttpContextAccessor = iHttpContextAccessor;
            this._iClaimService = iClaimService;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.Byte[]")]
        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var claimType = _configuration.GetSection(CmsClaimType.ClaimType);
            var controllerActionType = claimType.GetValue<string>(CmsClaimType.ControllerAction);
            if (principal.Identity == null) return principal;
            this._iClaimService.ReloadInfoUser(principal, user);
            var listControllerAction =
                _iClaimUserRepository.GetControllerActionRoleByUserClaimType(user.Id, controllerActionType);
            if (listControllerAction.Count <= 0) return principal;
            listControllerAction.ForEach(x =>
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(controllerActionType, x.ToUpper()));
            });
            var rs = _iClaimUserRepository.GetMenuByUserAndControllerAction(listControllerAction);
            if (rs.Count > 0)
            {
                this._iHttpContextAccessor.HttpContext?.Session.SetString(CmsClaimType.Menu, JsonService.SerializeObject(rs));
            }

            return principal;
        }
    }
}