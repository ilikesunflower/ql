using System.Linq;
using System.Net;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger;
        }
        public IActionResult OnGet(string returnUrl)
        {
            this._logger.LogInformation($"Tài khoản {HttpContext.User.Identity?.Name} đăng xuất thành công");
            HttpContext.Session.Clear();
            return SignOut(new AuthenticationProperties() { RedirectUri = "/Identity/Account/Login" },
                    new[] {CookieAuthenticationDefaults.AuthenticationScheme, IdentityConstants.ExternalScheme,IdentityConstants.TwoFactorUserIdScheme,
                        IdentityConstants.ApplicationScheme });
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPost(string returnUrl = null)
        {
            this._logger.LogInformation($"Tài khoản {HttpContext.User.Identity?.Name} đăng xuất thành công");
            HttpContext.Session.Clear();
            return SignOut(new AuthenticationProperties() { RedirectUri = "/Identity/Account/Login" },
                    new[] {CookieAuthenticationDefaults.AuthenticationScheme, IdentityConstants.ExternalScheme,IdentityConstants.TwoFactorUserIdScheme,
                        IdentityConstants.ApplicationScheme });
        }
    }
}