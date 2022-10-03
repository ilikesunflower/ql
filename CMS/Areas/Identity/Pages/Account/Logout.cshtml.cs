using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Identity.Pages.Account
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IHttpContextAccessor _iHttpContextAccessor;

        public LogoutModel(ILogger<LogoutModel> logger,IHttpContextAccessor iHttpContextAccessor)
        {
            _logger = logger;
            _iHttpContextAccessor = iHttpContextAccessor;
        }
        [IgnoreAntiforgeryToken]
        public IActionResult OnGet(string returnUrl)
        {
            try
            {
                HttpContext.Session.Clear();
                if (User.Identity!.IsAuthenticated)
                {
                    this._logger.LogInformation($"Tài khoản {HttpContext.User.Identity?.Name} đăng xuất thành công");
                    return SignOut(new AuthenticationProperties() { RedirectUri = "/Identity/Account/Login" },
                        new[] {CookieAuthenticationDefaults.AuthenticationScheme, IdentityConstants.ExternalScheme,IdentityConstants.TwoFactorUserIdScheme,
                            IdentityConstants.ApplicationScheme });   
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return Redirect("/Identity/Account/Login");
        }

        [AutoValidateAntiforgeryToken]
        public IActionResult OnPost(string returnUrl = null)
        {
            try
            {
                HttpContext.Session.Clear();
                if (User.Identity!.IsAuthenticated)
                {
                    this._logger.LogInformation($"Tài khoản {HttpContext.User.Identity?.Name} đăng xuất thành công");
                    return SignOut(new AuthenticationProperties() { RedirectUri = "/Identity/Account/Login" },
                        new[] {CookieAuthenticationDefaults.AuthenticationScheme, IdentityConstants.ExternalScheme,IdentityConstants.TwoFactorUserIdScheme,
                            IdentityConstants.ApplicationScheme });   
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return Redirect("/Identity/Account/Login");
        }
    }
}